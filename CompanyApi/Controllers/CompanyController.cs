using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : Controller
    {
        
        private static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companies.Exists(_ => _.Name.Equals(company.Name)))
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }

        [HttpDelete]
        public void DeleteAllPets()
        {
            companies.Clear();
        }
    }
}

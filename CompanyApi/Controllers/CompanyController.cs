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
                return Conflict();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return Created($"/companies/{company.CompanyID}", company);
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAllPets()
        {
            return Ok(companies);
        }

        [HttpGet("{companyID}")]
        public ActionResult<Company> GetAllPets([FromRoute] string companyID)
        {
            return Ok(companies.Find(_ => _.CompanyID.Equals(companyID)));
        }

        [HttpDelete]
        public ActionResult DeleteAllPets()
        {
            companies.Clear();
            return Ok();
        }
    }
}

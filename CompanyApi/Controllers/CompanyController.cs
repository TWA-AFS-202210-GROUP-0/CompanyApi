using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> CreateCompany([FromBody] Company company)
        {
            if (companies.Any(c => c.Name.Equals(company.Name)))
            {
                return Conflict($"company {company.Name} already exist");
            }

            company.Id = System.Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult("companies/{company.Id}", company);
        }

        [HttpDelete("deleteAll")]
        public void DeleteCompany()
        {
            companies.Clear();
        }
    }
}
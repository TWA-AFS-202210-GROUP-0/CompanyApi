using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpDelete("all")]
        public void DeleteCompanies()
        {
            companies.Clear();
        }

        [HttpGet("all")]
        public List<Company> GetAllCompanies()
        {
            return companies;
        }

        [HttpGet]
        [Route("{Id}")]
        public ActionResult<Company> GetCompanyById([FromRoute] string id)
        {
            try
            {
                var res = companies.Single(c => c.Id.Equals(id));
                return new ObjectResult(res);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
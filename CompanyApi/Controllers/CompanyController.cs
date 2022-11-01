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
        public ActionResult<List<Company>> GetAllCompanies()
        {
            return Ok(companies);
        }

        [HttpGet("{companyID}")]
        public ActionResult<Company> GetCompany([FromRoute] string companyID)
        {
            return Ok(companies.Find(_ => _.CompanyID.Equals(companyID)));
        }

        [HttpGet("page/{pageIndex}")]
        public ActionResult<Company> GetCompany([FromRoute] int pageIndex, [FromQuery] int pageSize)
        {
            int companyCount = companies.Count();
            int beginCompanyIndex = (pageIndex * pageSize) - 1;
            int pageIndexCount = Math.Min(pageSize, companyCount - (pageIndex * pageSize));
            if (pageIndexCount <= 0)
            {
                return NotFound();
            }

            return Ok(companies.GetRange(beginCompanyIndex, pageIndexCount));
        }

        [HttpPut("{updateCompany.CompanyID}")]
        public ActionResult<Company> ModifyCompanyName(Company updateCompany)
        {
            var currentCompany = companies.Find(_ => _.CompanyID == updateCompany.CompanyID);
            if (currentCompany == null)
            {
                return NotFound();
            }

            currentCompany.Name = updateCompany.Name;
            return Ok(currentCompany);
        }

        [HttpDelete]
        public ActionResult DeleteAllPets()
        {
            companies.Clear();
            return Ok();
        }
    }
}

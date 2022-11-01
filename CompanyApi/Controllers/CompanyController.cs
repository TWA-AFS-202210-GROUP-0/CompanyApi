using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CompanyApi.Models;
namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();
        [HttpPost("companies")]
        public ActionResult<Company> AddNewCompany([FromBody] Company company)
        {
            if (companies.Where(cmp => cmp.Name == company.Name).ToList().Count == 0)
            {
                company.CompanyId = Guid.NewGuid().ToString();
                companies.Add(company);
                return new CreatedResult($"/companies/{company.CompanyId}", company);
            }
            else
            {
                return new ConflictResult();
            }

            //throw new CompanyException("the company has existed!");
        }

        [HttpGet("getAllCompanies")]
        public List<Company> GetAllCompanies()
        {
            return companies;
        }

        [HttpDelete("deletAllCompanies")]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companies.Exists(p => string.Equals(company.Name, p.Name)))
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }

        [HttpGet]
        public List<Company> GetAllCompany([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            {
                // return companies.GetRange((pageIndex.Value - 1) * pageSize.Value, pageSize.Value);
                return companies
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return companies;
        }

        [HttpGet("{id}")]
        public Company GetCompanyById([FromRoute] string id)
        {
            return companies.Find(_ => _.CompanyID == id);
        }

        [HttpPut]
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

        //[HttpPut]
        //public ActionResult<Company> Put(Company updateCompany)
        //{
        //    return updateCompany;
        //}

        [HttpDelete]
        public void DeleteAllPets()
        {
            companies.Clear();
        }
    }
}

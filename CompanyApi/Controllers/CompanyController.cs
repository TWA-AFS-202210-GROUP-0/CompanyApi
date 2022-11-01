using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        [HttpPost("companies/{id}/employees")]
        public ActionResult<Employee> AddNewEmployee([FromRoute] string id, [FromBody] Employee employee)
        {
            var company = companies.Find(cmp => cmp.CompanyId == id);
            if (company != null)
            {
                employee.EmployeeId = Guid.NewGuid().ToString();
                company.Employees.Add(employee);
                return new CreatedResult($"/companies/{id}/employees/{employee.EmployeeId}", employee);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("companies/{id}/employees")]
        public List<Employee> GetAllEmployee([FromRoute] string id)
        {
            var company = companies.Find(cmp => cmp.CompanyId == id);
            return company.Employees;
        }

        [HttpGet("companies/{id}")]
        public Company GetOneCompany([FromRoute] string id)
        {
            return companies.Find(cmp => cmp.CompanyId == id);
        }

        [HttpPut("companies/{id}")]
        public Company UpdateOneCompany([FromRoute] string id, [FromBody] Company company)
        {
            var cmp = companies.Find(cmp => cmp.CompanyId == id);
            cmp.Name = company.Name;
            return cmp;
        }

        [HttpGet("companies")]
        public List<Company> GetSeveralCompaniesFromOnePage([FromQuery] int? pageIndex, [FromQuery] int? pageSize)
        {
            if (pageIndex != null && pageSize != null)
            {
                int lower = (pageIndex.Value - 1) * pageSize.Value;
                int upper = lower + pageSize.Value;
                if (upper <= companies.Count)
                {
                    return companies.Skip(lower).Take(pageSize.Value).ToList();
                }
                else
                {
                    return companies.Skip(lower).Take(pageSize.Value - upper + companies.Count).ToList();
                }
            }
            else
            {
                return companies;
            }
        }

        [HttpDelete("companies")]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }
    }
}

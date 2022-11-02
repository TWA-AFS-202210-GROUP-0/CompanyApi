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
        internal static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany([FromBody] Company company)
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
        public ActionResult<Company> ModifyCompanyName([FromBody] Company updateCompany)
        {
            var currentCompany = companies.Find(_ => _.CompanyID == updateCompany.CompanyID);
            if (currentCompany == null)
            {
                return NotFound();
            }

            currentCompany.Name = updateCompany.Name;
            return Ok(currentCompany);
        }

        [HttpPost("{id}/employees")]
        public ActionResult<Employee> AddNewEmployee([FromRoute] string id, [FromBody] Employee employee)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            var employees = company.Employees;
            if (employees.Exists(p => string.Equals(employee.Name, p.Name)))
            {
                return new ConflictResult();
            }

            employee.EmployeeID = Guid.NewGuid().ToString();
            employees.Add(employee);
            return new CreatedResult($"/companies/{employee.EmployeeID}", employee);
        }

        [HttpGet("{id}/employees")]
        public ActionResult<List<Employee>> GetAllEmployee([FromRoute] string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            if (company == null)
            {
                return NotFound();
            }

            var employees = company.Employees;
            return employees;
        }

        [HttpPut("{id}/employees")]
        public ActionResult<Employee> ModifyCompanyName([FromBody] Employee employee, [FromRoute] string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            var employees = company.Employees;
            var currentEmployee = employees.Find(_ => _.EmployeeID == employee.EmployeeID);
            if (currentEmployee == null)
            {
                return NotFound();
            }

            currentEmployee.Name = employee.Name;
            return Ok(currentEmployee);
        }

        [HttpDelete("{id}/employees/{name}")]

        public ActionResult<Employee> DeleteEmployeeByName([FromRoute] string name, [FromRoute] string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            if (company == null)
            {
                return NotFound();
            }

            var employees = company.Employees;
            var currentEmployee = employees.Find(_ => _.Name == name);
            if (currentEmployee == null)
            {
                return NotFound();
            }

            employees.Remove(currentEmployee);
            return Ok(currentEmployee);
        }

        [HttpDelete("{id}")]
        public ActionResult<Company> DeleteCompanyById([FromRoute] string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            if (company == null)
            {
                return NotFound();
            }

            var employees = company.Employees;
            employees.Clear();
            companies.Remove(company);
            return Ok(company);
        }

        [HttpDelete]
        public void DeleteAllCompany()
        {
            companies.Clear();
        }

        [HttpDelete("{id}/employees")]
        public ActionResult<List<Employee>> DeleteAllEmployeeOfACompany([FromRoute] string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            if (company == null)
            {
                return NotFound();
            }

            var employees = company.Employees;
            employees.Clear();
            return Ok(employees);
        }
    }
}

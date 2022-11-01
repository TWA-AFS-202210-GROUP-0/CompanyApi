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

        [HttpDelete]
        public void DeleteAllPets()
        {
            companies.Clear();
        }

        [HttpPost("{id}/employees")]
        public ActionResult<Employee> AddNewEmployee(Employee employee, string id)
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
        public ActionResult<List<Employee>> GetAllEmployee(string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            var employees = company.Employees;
            return employees;
        }

        [HttpPut("{id}/employees")]
        public ActionResult<Employee> ModifyCompanyName(Employee employee, string id)
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

        public ActionResult<Employee> DeleteEmployeeByName(string name, string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
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
        public ActionResult<Company> DeleteCompanyById(string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            var employees = company.Employees;
            employees.Clear();
            companies.Remove(company);
            return Ok(company);
        }

        [HttpDelete]
        public void DeleteAllPets(string id)
        {
            var company = companies.Find(_ => _.CompanyID == id);
            var employees = company.Employees;
            employees.Clear();
        }
    }
}

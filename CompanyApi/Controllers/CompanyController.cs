using CompanyApiTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companies.Exists(comp => comp.Name.Equals(company.Name))) { return new ConflictResult();  }
            company.Id = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.Id}", company);
        }

        [HttpGet]
        public ActionResult<List<Company>> GetCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageIndex != null && pageSize != null)
            {
                int start = (int)((pageIndex - 1) * pageSize);
                int end = (int)((start + pageSize) > companies.Count ? companies.Count : (start + pageSize));
                return companies.GetRange(start, end);
            }

            return companies;
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet("{id}")]
        public ActionResult<Company> GetCompany([FromRoute] string id)
        {
            var company = companies.FirstOrDefault(comp => comp.Id.Equals(id));
            return company == null ? NotFound() : company;
        }

        [HttpPut("{id}")]
        public ActionResult<Company> UpdateCompanyInfo([FromRoute] string id, Company company)
        {
            var originalCompany = companies.FirstOrDefault(comp => comp.Id.Equals(id));
            if (originalCompany == null) { return NotFound(); }
            originalCompany.Name = company.Name;
            return originalCompany;
        }

        [HttpPost("{id}/employees")]
        public ActionResult<Employee> AddEmployee([FromRoute] string id, Employee employee)
        {
            var company = companies.FirstOrDefault(comp => comp.Id.Equals(id));
            if (company == null) { return NotFound(); }
            return company.AddEmployee(employee) ? new CreatedResult($"/companies/{company.Id}/employees/{employee.Id}", employee) : StatusCode(500);
        }

        [HttpGet("{id}/employees")]
        public ActionResult<List<Employee>> GetEmployees([FromRoute] string id)
        {
            return companies.FirstOrDefault(comp => comp.Id.Equals(id))?.Employees;
        }

        [HttpPut("{companyId}/employees/{employeeId}")]
        public ActionResult<Employee> UpdateEmployee([FromRoute] string companyId, [FromRoute] string employeeId, Employee newEmployee)
        {
            var employees = companies.FirstOrDefault(comp => comp.Id.Equals(companyId))?.Employees;
            var employee = employees.FirstOrDefault(employee => employee.Id.Equals(employeeId));
            employee = newEmployee;
            return employee;
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public ActionResult DeleteEmployee([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            var employees = companies.FirstOrDefault(comp => comp.Id.Equals(companyId))?.Employees;
            var employee = employees.FirstOrDefault(employee => employee.Id.Equals(employeeId));
            if (employee == null) { return Conflict(); }
            employees.Remove(employee);
            return Ok();
        }
    }
}

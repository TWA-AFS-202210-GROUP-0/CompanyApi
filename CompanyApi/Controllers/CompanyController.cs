using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CompanyApi.Dto;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpDelete("")]
        public void DeleteAll()
        {
            companies.Clear();
        }

        [HttpPost("")]
        public ActionResult<CompanyDto> CreateCompany([FromBody] CompanyDto companyDto)
        {
            if (companies.Exists(e => e.Name.Equals(companyDto.Name)))
            {
                return new ConflictResult();
            }

            var newCompany = new Company()
            {
                Name = companyDto.Name,
                CompanyId = Guid.NewGuid().ToString(),
            };
            companies.Add(newCompany);

            return new CreatedResult($"/companies/{newCompany.CompanyId}", newCompany.ToDto());
        }

        [HttpGet("{id}")]
        public ActionResult<CompanyDto> GetCompany([FromRoute] string id)
        {
            var companyByFind = companies.Find(e => e.CompanyId == id);
            if (companyByFind != null)
            {
                return Ok(companyByFind.ToDto());
            }

            return new NotFoundResult();
        }

        [HttpGet("")]
        public ActionResult<List<CompanyDto>> GetCompaniesByPage([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (!pageSize.HasValue || !pageIndex.HasValue)
            {
                var companiesDto = companies.Select(company => company.ToDto()).ToList();
                return Ok(companiesDto);
            }

            var startIndex = pageSize * (pageIndex - 1);
            var endIndex = startIndex + pageSize - 1;
            if (startIndex > companies.Count)
            {
                return new NotFoundResult();
            }

            pageSize = endIndex < companies.Count ? pageSize : companies.Count - startIndex;
            var companiesInPage = companies.GetRange(startIndex.GetValueOrDefault(), pageSize.GetValueOrDefault())
                .ToList();
            var companiesInPageDto = companiesInPage.Select(company => company.ToDto()).ToList();
            return Ok(companiesInPageDto);
        }

        [HttpPatch("{companyId}")]
        public ActionResult<CompanyDto> UpdateCompanyInformation([FromRoute] string companyId,
            [FromBody] CompanyDto companyDto)
        {
            var companyToBeUpdated = companies.Find(e => e.CompanyId == companyId);
            if (companyToBeUpdated != null)
            {
                companyToBeUpdated.Name = companyDto.Name;
                return Ok(companyToBeUpdated.ToDto());
            }

            return new NotFoundResult();
        }

        [HttpPost("{companyId}/employees/")]
        public ActionResult<EmployeeDto> AddEmployee([FromRoute] string companyId, [FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto.Name == null)
            {
                return BadRequest();
            }

            var companyByFind = companies.Find(e => e.CompanyId == companyId);
            if (companyByFind == null)
            {
                return new NotFoundResult();
            }

            const double initSalary = 100;
            var newEmployee = new Employee()
            {
                Name = employeeDto.Name,
                Salary = employeeDto.Salary.HasValue ? employeeDto.Salary.Value : initSalary,
                EmployeeId = Guid.NewGuid().ToString(),
            };

            companyByFind.Employees.Add(newEmployee);

            return new CreatedResult($"/companies/{companyByFind.CompanyId}/employees/{newEmployee.EmployeeId}",
                newEmployee.ToDto());
        }

        [HttpGet("{companyId}/employees/")]
        public ActionResult<List<EmployeeDto>> GetEmployees([FromRoute] string companyId)
        {
            var companyByFind = companies.Find(e => e.CompanyId == companyId);
            if (companyByFind == null)
            {
                return new NotFoundResult();
            }

            return Ok(companyByFind.Employees.Select(e => e.ToDto()).ToList());
        }

        [HttpGet("{companyId}/employees/{employeeId}")]
        public ActionResult<List<EmployeeDto>> GetEmployees([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            var companyByFind = companies.Find(e => e.CompanyId == companyId);
            if (companyByFind != null)
            {
                var employeebyFind = companyByFind.Employees.Find(e => e.EmployeeId == employeeId);
                if (employeebyFind != null)
                {
                    return Ok(employeebyFind.ToDto());
                }
            }

            return new NotFoundResult();
        }

        [HttpPatch("{companyId}/employees/{employeeId}")]
        public ActionResult<CompanyDto> UpdateEmployeeInformation([FromRoute] string companyId,
            [FromRoute] string employeeId, [FromBody] EmployeeDto employeeDto)
        {
            var companyByFind = companies.Find(e => e.CompanyId == companyId);
            if (companyByFind != null)
            {
                var employeToBeUpdated = companyByFind.Employees.Find(e => e.EmployeeId.Equals(employeeId));
                if (employeToBeUpdated != null)
                {
                    if (employeeDto.Name != null)
                    {
                        employeToBeUpdated.Name = employeeDto.Name;
                    }

                    if (employeeDto.Salary.HasValue)
                    {
                        employeToBeUpdated.Salary = employeeDto.Salary.Value;
                    }

                    return Ok(employeToBeUpdated.ToDto());
                }
            }

            return new NotFoundResult();
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public ActionResult<string> DeleteEmployee([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            var companyByFind = companies.Find(e => e.CompanyId == companyId);
            if (companyByFind != null)
            {
                var employeToBeRemoved = companyByFind.Employees.Find(e => e.EmployeeId.Equals(employeeId));
                if (employeToBeRemoved != null)
                {
                    companyByFind.Employees.Remove(employeToBeRemoved);
                    return Ok("Employee removed");
                }
            }

            return new NotFoundResult();
        }

        [HttpDelete("{companyId}")]
        public ActionResult<string> DeleteCompany([FromRoute] string companyId)
        {
            var companyByFind = companies.Find(e => e.CompanyId == companyId);
            if (companyByFind != null)
            {
                companies.Remove(companyByFind);
                return Ok("Company removed");
            }

            return new NotFoundResult();
        }
    }
}
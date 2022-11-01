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
        public ActionResult<List<CompanyDto>> GetCompaniesByPage([FromQuery] int? pageSize, [FromQuery]int? pageIndex)
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
            var companiesInPage = companies.GetRange(startIndex.GetValueOrDefault(), pageSize.GetValueOrDefault()).ToList();
            var companiesInPageDto = companiesInPage.Select(company => company.ToDto()).ToList();
            return Ok(companiesInPageDto);
        }

        [HttpPatch("{companyId}")]
        public ActionResult<CompanyDto> UpdateCompanyInformation([FromRoute] string companyId, [FromBody] CompanyDto companyDto)
        {
            var companyToBeUpdated = companies.Find(e => e.CompanyId == companyId);
            if (companyToBeUpdated != null)
            {
                if (companyDto.Name != null)
                {
                    companyToBeUpdated.Name = companyDto.Name;
                }

                return Ok(companyToBeUpdated.ToDto());
            }

            return new NotFoundResult();
        }

        [HttpPost("{companyId}/employees/")]
        public ActionResult<EmployeeDto> AddEmployee([FromRoute] string companyId, [FromBody] EmployeeDto employeeDto)
        {
            var companyByFind = companies.Find(e => e.CompanyId == companyId);
            if (companyByFind == null)
            {
                return new NotFoundResult();
            }

            var newEmployee = new Employee()
            {
                Name = employeeDto.Name,
                Salary = employeeDto.Salary,
                EmployeeId = Guid.NewGuid().ToString(),
            };

            companyByFind.Employees.Add(newEmployee);

            return new CreatedResult($"/companies/{companyByFind.CompanyId}/employees/{newEmployee.EmployeeId}", newEmployee.ToDto());
        }
    }
}
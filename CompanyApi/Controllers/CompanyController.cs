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
        private static List<Employee> employees = new List<Employee>();

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

        [HttpGet]
        public ActionResult<List<Company>> GetCompaniesByPageIndexAndPageSize([FromQuery] string pageSize, [FromQuery] string pageIndex)
        {
            try
            {
                int pageSizeInt = Convert.ToInt32(pageSize);
                int pageIndexInt = Convert.ToInt32(pageIndex);
                return companies.Skip(pageSizeInt * (pageIndexInt - 1)).Take(pageSizeInt).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("{Id}")]
        public ActionResult<Company> UpdateCompany([FromRoute] string id, [FromBody] Company company)
        {
            try
            {
                var curCompany = companies.Single(c => c.Id.Equals(id));
                curCompany.Name = company.Name;

                return Ok(curCompany);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{Id}/employee")]
        public ActionResult<Company> AddEmployee([FromRoute] string id, [FromBody] Employee employee)
        {
            try
            {
                var curCompany = companies.Single(c => c.Id.Equals(id));
                employee.CompanyId = curCompany.Id;
                var employeeIds = curCompany.EmployeeIDs;
                if (employeeIds?.Count > 0)
                {
                    employeeIds.Add(employee.EmployeeID);
                }
                else
                {
                    curCompany.EmployeeIDs = new List<string> { employee.EmployeeID };
                }

                employees.Add(employee);
                return Ok(curCompany);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("{Id}/all/employee")]
        public ActionResult<List<Employee>> GetAllEmployeeUnderACompany([FromRoute] string id)
        {
            try
            {
                var res = companies.Single(c => c.Id.Equals(id));
                return Ok(employees.Where(e => e.CompanyId.Equals(id)));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
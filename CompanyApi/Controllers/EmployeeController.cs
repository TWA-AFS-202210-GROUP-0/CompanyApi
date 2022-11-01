using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies/{id}/employees")]
    public class EmployeeController : ControllerBase
    {
        private static List<Employee> employees = new List<Employee>();
        [HttpPost]
        public ActionResult<Employee> AddNewEmployee(Employee employee)
        {
            if (employees.Exists(p => string.Equals(employee.Name, p.Name)))
            {
                return new ConflictResult();
            }

            employee.EmployeeID = Guid.NewGuid().ToString();
            employees.Add(employee);
            return new CreatedResult($"/companies/{employee.EmployeeID}", employee);
        }
    }
}

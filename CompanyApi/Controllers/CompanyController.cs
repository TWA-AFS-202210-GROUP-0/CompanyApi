using CompanyApiTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost("companies")]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            company.Id = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.Id}", company);
        }
    }
}

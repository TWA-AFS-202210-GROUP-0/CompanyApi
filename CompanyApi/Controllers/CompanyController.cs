﻿using CompanyApiTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
        public ActionResult<List<Company>> GetCompanies()
        {
            return companies;
        }
    }
}

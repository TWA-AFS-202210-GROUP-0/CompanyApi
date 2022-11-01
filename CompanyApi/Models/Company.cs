using System;
using System.Collections.Generic;

namespace CompanyApiTest.Controllers
{
    public class Company
    {

        public Company(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string? Id { get; set; }
    }
}
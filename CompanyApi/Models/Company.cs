using System;
using System.Collections.Generic;

namespace CompanyApiTest.Controllers
{
    public class Company
    {
        private string name;

        public Company(string name)
        {
            this.name = name;
        }

        public string Name { get => name; }
        public string? Id { get; set; }
    }
}
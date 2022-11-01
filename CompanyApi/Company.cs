using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyApi
{
    public class Company
    {   
        public Company(string name)
        {
            CompanyID = string.Empty;
            Name = name;
            Employees = new List<Employee>();
        }

        public string Name { get; set; }
        public string? CompanyID { get; set; }
        public List<Employee> Employees { get; set; }
    }
}

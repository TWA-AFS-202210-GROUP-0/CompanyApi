using System;
using System.Collections.Generic;

namespace CompanyApi.Models
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            CompanyID = string.Empty;
            Employees = new List<Employee>();
        }

        public string? CompanyID { get; set; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
    }
}

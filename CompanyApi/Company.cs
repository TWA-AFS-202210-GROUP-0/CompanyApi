using Microsoft.AspNetCore.Mvc;
using System;

namespace CompanyApi
{
    public class Company
    {   
        public Company(string name)
        {
            CompanyID = string.Empty;
            Name = name;
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
    }
}

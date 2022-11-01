using System;

namespace CompanyApi.Models
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            CompanyID = string.Empty;
        }

        public string? CompanyID { get; set; }
        public string Name { get; set; }
    }
}

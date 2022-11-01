using System;
using System.Collections.Generic;

namespace CompanyApi
{
    public class Company
    {
        public Company(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
        public string? Id { get; set; }

        public List<string>? EmployeeIDs { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Company company &&
                   Name == company.Name &&
                   Id == company.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Id);
        }
    }
}
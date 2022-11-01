using System;

namespace CompanyApi.Models
{
    public class Company
    {
        private string name;
        private string companyId;
        public Company(string name)
        {
            this.Name = name;
            this.CompanyId = string.Empty;
        }

        public string Name { get => name; set => name = value; }
        public string CompanyId { get => companyId; set => companyId = value; }

        public override bool Equals(object? obj)
        {
            var other = obj as Company;
            return other != null && other.Name == this.Name;
        }
    }
}

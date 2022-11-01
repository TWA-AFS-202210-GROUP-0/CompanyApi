using System.Collections.Generic;
using System.Linq;
using CompanyApi.Dto;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CompanyApi.Model
{
    public class Company
    {
        public string CompanyId { get; init; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();

        public CompanyDto ToDto()
        {
            return new CompanyDto
            {
                Name = this.Name,
                CompanyId = this.CompanyId,
            };
        }

        public CompanyDto ToDtoWithEmployees()
        {
            return new CompanyDto
            {
                Name = this.Name,
                CompanyId = this.CompanyId,
                Employees = this.Employees.Select(e => e.ToDto()).ToList(),
            };
        }
    }
}

using System;
using System.Collections.Generic;

namespace CompanyApiTest.Controllers
{
    public class Company
    {
        private List<Employee> employees;
        public Company(string name)
        {
            Name = name;
            employees = new List<Employee>();
        }

        public string Name { get; set; }
        public string? Id { get; set; }

        public List<Employee> Employees { get => employees; }

        public bool AddEmployee(Employee employee)
        {
            try
            {
                employee.Id = Guid.NewGuid().ToString();
                employees.Add(employee);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
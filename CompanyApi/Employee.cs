using System;

namespace CompanyApi
{
    public class Employee
    {
        public Employee(string name)
        {
            this.Name = name;
            this.EmployeeID = Guid.NewGuid().ToString();
        }

        public string EmployeeID { get; set; }
        public string? Name { get; set; }

        public double? Salary { get; set; }

        public string? CompanyId { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Employee employee &&
                   EmployeeID == employee.EmployeeID &&
                   Name == employee.Name &&
                   Salary == employee.Salary &&
                   CompanyId == employee.CompanyId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EmployeeID, Name, Salary, CompanyId);
        }
    }
}
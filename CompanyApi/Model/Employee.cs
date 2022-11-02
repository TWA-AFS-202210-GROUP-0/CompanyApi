using CompanyApi.Dto;

namespace CompanyApi.Model
{
    public class Employee
    {
        public string Name { get; set; }
        public double Salary { get; set; }
        public string EmployeeId { get; set; }

        public EmployeeDto ToDto()
        {
            return new EmployeeDto
            {
                Name = this.Name,
                Salary = this.Salary,
                EmployeeId = this.EmployeeId
            };
        }

        public double CalculateSalary()
        {
            this.Salary = 1000 * this.Name.Length;
            return this.Salary;
        }
    }
}

namespace CompanyApi.Models
{
     public class Employee
    {
        public Employee(string name, int salary)
        {
            this.Name = name;
            this.Salary = salary;
            this.EmployeeId = string.Empty;
        }

        public string Name { get; }
        public int Salary { get; }
        public string EmployeeId { get; set; }

        public override bool Equals(object? obj)
        {
            var other = obj as Employee;
            return other != null && other.Name == this.Name && other.Salary == this.Salary;
        }
    }
}
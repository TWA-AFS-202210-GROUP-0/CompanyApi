namespace CompanyApi
{
    public class Employee
    {
        public Employee (string name, int salary)
        {
            EmployeeID = string.Empty;
            Name = name;
            Salary = salary;
        }

        public string Name { get; set; }
        public string? EmployeeID { get; set; }
        public int Salary { get; set; }
    }
}
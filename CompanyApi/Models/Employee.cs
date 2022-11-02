namespace CompanyApi.Models
{
    public class Employee
    {
        public Employee(string name)
        {
            Name = name;
            EmployeeID = string.Empty;
        }

        public string? EmployeeID { get; set; }
        public string Name { get; set; }
    }
}

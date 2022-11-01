namespace CompanyApiTest.Controllers
{
    public class Employee
    {
        public Employee(string name, float salary)
        {
            Name = name;
            Salary = salary;
        }

        public string Name { get; set; }
        public float Salary { get; set; }
        public string? Id { get; set; }
    }
}
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
    }
}
using System.Collections.Generic;

namespace CompanyApi.Dto
{
    public class CompanyDto
    {
        public string Name { get; set; }
        public string? CompanyId { get; set; }
        public List<EmployeeDto>? Employees { get; set; }
    }
}

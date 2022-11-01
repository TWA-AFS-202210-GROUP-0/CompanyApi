using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        [HttpPost]
        public Company CreateCompany([FromBody] Company company)
        {
            company.Id = System.Guid.NewGuid().ToString();
            return company;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class EmployeeControllerTest
    {
        [Fact]
        public async Task Should_add_employee_to_company_with_id_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            string comparyId = createdCompany.CompanyID;
            var employee = new Employee(name: "lijie");
            StringContent postEmployeeBody = BuildRequestBody(employee);
            // when
            var employeeResponse = await httpClient.PostAsync($"/companies/{comparyId}/employees", postEmployeeBody);
            // then
            // then
            Assert.Equal(HttpStatusCode.Created, employeeResponse.StatusCode);
            var employeeResponseBody = await employeeResponse.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(employeeResponseBody);
            Assert.NotEmpty(createdEmployee.EmployeeID);
        }

        public static StringContent BuildRequestBody(Object newCompany)
        {
            var companyJson = JsonConvert.SerializeObject(newCompany);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return postBody;
        }
    }
}

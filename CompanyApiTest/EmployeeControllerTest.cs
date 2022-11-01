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
            await httpClient.DeleteAsync("/companies/{comparyId}/employees");
            // when
            var employeeResponse = await httpClient.PostAsync($"/companies/{comparyId}/employees", postEmployeeBody);
            // then
            // then
            Assert.Equal(HttpStatusCode.Created, employeeResponse.StatusCode);
            var employeeResponseBody = await employeeResponse.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(employeeResponseBody);
            Assert.NotEmpty(createdEmployee.EmployeeID);
        }

        [Fact]
        public async Task Should_get_employee_list_of_a_company_with_id_successfully()
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
            string companyId = createdCompany.CompanyID;
            await httpClient.DeleteAsync("/companies/{comparyId}/employees");
            var employee = new Employee(name: "lijie");
            StringContent postEmployeeBody = BuildRequestBody(employee);
            var employeeResponse = await httpClient.PostAsync($"/companies/{companyId}/employees", postEmployeeBody);
            var employee1 = new Employee(name: "liwenrui");
            StringContent postEmployeeBody1 = BuildRequestBody(employee1);
            var employeeResponse1 = await httpClient.PostAsync($"/companies/{companyId}/employees", postEmployeeBody1);
            var employee2 = new Employee(name: "songsiqi");
            StringContent postEmployeeBody2 = BuildRequestBody(employee2);
            var employeeResponse2 = await httpClient.PostAsync($"/companies/{companyId}/employees", postEmployeeBody2);
            // when
            var getAllEmployeeResponse = await httpClient.GetAsync($"/companies/{companyId}/employees");
            // then
            Assert.Equal(HttpStatusCode.OK, getAllEmployeeResponse.StatusCode);
        }

        [Fact]
        public async void Should_update_employee_information_of_an_existing_employee_of_a_company_successfully()
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
            await httpClient.DeleteAsync("/companies/{comparyId}/employees");
            var employeeResponse = await httpClient.PostAsync($"/companies/{comparyId}/employees", postEmployeeBody);
            var employeeResponseBody = await employeeResponse.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(employeeResponseBody);
            createdEmployee.Name = "jieli";
            StringContent newNamedBody = BuildRequestBody(createdEmployee);
            // when
            var newNamedEmployeeResponse = await httpClient.PutAsync($"/companies/{comparyId}/employees", newNamedBody);
            // then
            Assert.Equal(HttpStatusCode.OK, newNamedEmployeeResponse.StatusCode);
            var newNamedEmployeeBody = await newNamedEmployeeResponse.Content.ReadAsStringAsync();
            var newNamedEmployee = JsonConvert.DeserializeObject<Employee>(newNamedEmployeeBody);
            Assert.Equal("jieli", newNamedEmployee.Name);
        }

        public static StringContent BuildRequestBody(Object newCompany)
        {
            var companyJson = JsonConvert.SerializeObject(newCompany);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return postBody;
        }
    }
}

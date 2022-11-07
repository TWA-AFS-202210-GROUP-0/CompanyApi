using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest : IDisposable
    {
        private readonly HttpClient httpClient;
        public CompanyControllerTest()
        {
            var application = new WebApplicationFactory<Program>();
            httpClient = application.CreateClient();
        }

        public void Dispose()
        {
            httpClient.DeleteAsync("/companies");
        }

        [Fact]
        public async void Should_add_new_company_successfully()
        {
            // given
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdCompany = await DeserializeResponse<Company>(response);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async void Should_get_all_companies_from_system_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company1 = new Company(name: "SLB");
            StringContent company1Body = BuildRequestBody(company1);
            var company2 = new Company(name: "TW");
            StringContent company2Body = BuildRequestBody(company2);
            await httpClient.PostAsync("/companies", company1Body);
            await httpClient.PostAsync("/companies", company2Body);

            // when
            var response = await httpClient.GetAsync("/companies");

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var companiesList = await DeserializeResponse<List<Company>>(response);
            Assert.Equal(company1.Name, companiesList[0].Name);
            Assert.Equal(company2.Name, companiesList[1].Name);
        }

        [Fact]
        public async void Should_return_conflict_when_add_new_company_again()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            // when
            await httpClient.PostAsync("/companies", postBody);
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_existing_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);

            // when
            var createdResponse = await httpClient.PostAsync("/companies", postBody);
            var createdBody = await createdResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(createdBody);
            var getResponse = await httpClient.GetAsync($"/companies/{createdCompany.CompanyID}");

            // then
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getCompany = await DeserializeResponse<Company>(getResponse);
            Assert.Equal("SLB", getCompany.Name);
        }

        [Fact]
        public async void Should_get_companies_of_page_X_from_system_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company1 = new Company(name: "SLB");
            StringContent company1Body = BuildRequestBody(company1);
            var company2 = new Company(name: "TW");
            StringContent company2Body = BuildRequestBody(company2);
            var company3 = new Company(name: "SF");
            StringContent company3Body = BuildRequestBody(company3);
            await httpClient.PostAsync("/companies", company1Body);
            await httpClient.PostAsync("/companies", company2Body);
            await httpClient.PostAsync("/companies", company3Body);

            // when
            int pageSize = 1;
            int pageIndex = 2;
            var response = await httpClient.GetAsync($"/companies/page/{pageIndex}?pageSize={pageSize}");

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var companiesList = await DeserializeResponse<List<Company>>(response);
            Assert.Equal(company2.Name, companiesList[0].Name);
        }

        [Fact]
        public async void Should_get_companies_of_page_X_from_system_failed()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company1 = new Company(name: "SLB");
            StringContent company1Body = BuildRequestBody(company1);
            var company2 = new Company(name: "TW");
            StringContent company2Body = BuildRequestBody(company2);
            var company3 = new Company(name: "SF");
            StringContent company3Body = BuildRequestBody(company3);
            await httpClient.PostAsync("/companies", company1Body);
            await httpClient.PostAsync("/companies", company2Body);
            await httpClient.PostAsync("/companies", company3Body);

            // when
            int pageSize = 1;
            int pageIndex = 4;
            var response = await httpClient.GetAsync($"/companies/page/{pageIndex}?pageSize={pageSize}");

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Should_update_company_information_of_an_existing_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var response = await httpClient.PostAsync("/companies", postBody);
            var createdCompany = await DeserializeResponse<Company>(response);
            createdCompany.Name = "Schlumberger";
            StringContent updateBody = BuildRequestBody(createdCompany);

            //when
            var updateResponse = await httpClient.PutAsync($"/companies/{createdCompany.CompanyID}", updateBody);

            // then
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updatedCompany = await DeserializeResponse<Company>(updateResponse);
            Assert.Equal("Schlumberger", updatedCompany.Name);
        }

        [Fact]
        public async Task Should_add_an_employee_to_company_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var createdResponse = await httpClient.PostAsync("/companies", postBody);
            var createdCompany = await DeserializeResponse<Company>(createdResponse);
            var employee = new Employee(name: "Li Ming", salary: 10000);
            StringContent employeeBody = BuildRequestBody(employee);

            //when
            var response = await httpClient.PostAsync($"/companies/{createdCompany.CompanyID}/employees",
                employeeBody);

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updateResponse = await httpClient.GetAsync("/companies");
            var updatedCompanies = await DeserializeResponse<List<Company>>(updateResponse);
            Assert.Equal(employee.Name, updatedCompanies[0].Employees[0].Name);
        }

        [Fact]
        public async Task Should_get_all_employees_in_a_company_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var createdResponse = await httpClient.PostAsync("/companies", postBody);
            var createdCompany = await DeserializeResponse<Company>(createdResponse);
            var employee1 = new Employee(name: "Li Ming", salary: 10000);
            StringContent employeeBody1 = BuildRequestBody(employee1);
            await httpClient.PostAsync($"/companies/{createdCompany.CompanyID}/employees",
                employeeBody1);
            var employee2 = new Employee(name: "Li Hua", salary: 10000);
            StringContent employeeBody2 = BuildRequestBody(employee2);
            await httpClient.PostAsync($"/companies/{createdCompany.CompanyID}/employees",
                employeeBody2);

            //when
            var response = await httpClient.GetAsync($"/companies/{createdCompany.CompanyID}/employees");

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var employees = await DeserializeResponse<List<Employee>>(response);
            Assert.Equal(2, employees.Count);
        }

        [Fact]
        public async Task Should_update_an_employee_information_in_a_company_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var createdResponse = await httpClient.PostAsync("/companies", postBody);
            var createdBody = await createdResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(createdBody);
            var employee = new Employee(name: "Li Ming", salary: 10000);
            StringContent employeeBody = BuildRequestBody(employee);
            var response = await httpClient.PostAsync($"/companies/{createdCompany.CompanyID}/employees",
                employeeBody);
            var updateEmployee = await DeserializeResponse<Employee>(response);
            updateEmployee.Salary = 20000;
            StringContent updateBody = BuildRequestBody(updateEmployee);

            //when
            var updatedResponse = await httpClient.PutAsync($"/companies/{createdCompany.CompanyID}/employees/{updateEmployee.EmployeeID}", updateBody);

            //then
            Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
            var updatedEmployee = await DeserializeResponse<Employee>(updatedResponse);
            Assert.Equal(updateEmployee.Salary, updatedEmployee.Salary);
        }

        [Fact]
        public async Task Should_delete_an_employee_from_company_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var createdResponse = await httpClient.PostAsync("/companies", postBody);
            var createdCompany = await DeserializeResponse<Company>(createdResponse);
            var employee = new Employee(name: "Li Ming", salary: 10000);
            StringContent employeeBody = BuildRequestBody(employee);
            var response = await httpClient.PostAsync($"/companies/{createdCompany.CompanyID}/employees",
                employeeBody);
            var createEmployee = await DeserializeResponse<Employee>(response);
            //when
            var deleteResponse = await httpClient.DeleteAsync($"/companies/{createdCompany.CompanyID}/employees/{createEmployee.EmployeeID}");

            //then
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            var newEmployeesList = await DeserializeResponse<List<Employee>>(deleteResponse);
            Assert.Equal(0, newEmployeesList.Count);
        }

        [Fact]
        public async Task Should_delete_a_company_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var createdResponse = await httpClient.PostAsync("/companies", postBody);
            var createdCompany = await DeserializeResponse<Company>(createdResponse);
            var employee = new Employee(name: "Li Ming", salary: 10000);
            StringContent employeeBody = BuildRequestBody(employee);
            await httpClient.PostAsync($"/companies/{createdCompany.CompanyID}/employees",
                employeeBody);
            //when
            var deleteResponse = await httpClient.DeleteAsync($"/companies/{createdCompany.CompanyID}");

            //then
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        private static StringContent BuildRequestBody<T>(T requestObject)
        {
            var requestJson = JsonConvert.SerializeObject(requestObject);
            var requestBody = new StringContent(requestJson, Encoding.UTF8, "application/json");
            return requestBody;
        }

        private static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserializeObject = JsonConvert.DeserializeObject<T>(responseBody);

                return deserializeObject;
            }
            else
            {
                return default;
            }
        }

    }
}

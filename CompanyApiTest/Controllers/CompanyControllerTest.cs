using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        public HttpClient CreateHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            httpClient.DeleteAsync("/companies");
            return httpClient;
        }

        public StringContent CreateRequestBody(object obj)
        {
            var serializeObject = JsonConvert.SerializeObject(obj);
            return new StringContent(serializeObject, Encoding.UTF8, "application/json");
        }

        public async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public async Task<Company> AddCompany(HttpClient httpClient, string companyName)
        {
            var company = new Company(companyName);
            var requestBody = CreateRequestBody(company);
            var response = await httpClient.PostAsync("/companies", requestBody);
            Company createdCompany = await DeserializeResponse<Company>(response);
            return createdCompany;
        }

        public async Task<Employee> AddEmployee(HttpClient httpClient, string name, float salary, string companyID)
        {
            var employee = new Employee(name, salary);
            var requestBody = CreateRequestBody(employee);
            var response = await httpClient.PostAsync($"/companies/{companyID}/employees", requestBody);
            Employee createdEmployee = await DeserializeResponse<Employee>(response);
            return createdEmployee;
        }

        [Fact]
        public async Task Should_add_company_sucessfully()
        {
            // given
            var httpClient = CreateHttpClient();
            var company = new Company("Mengyao");
            var requestBody = CreateRequestBody(company);

            // when
            var response = await httpClient.PostAsync("/companies", requestBody);

            // then
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            Company createdCompany = await DeserializeResponse<Company>(response);
            Assert.Equal(company.Name, createdCompany.Name);
            Assert.NotEmpty(createdCompany.Id);
        }

        [Fact]
        public async Task Should_not_add_company_when_the_name_is_same()
        {
            // given
            var httpClient = CreateHttpClient();
            var company = new Company("Mengyao");
            var requestBody = CreateRequestBody(company);
            await httpClient.PostAsync("/companies", requestBody);

            // when
            var response = await httpClient.PostAsync("/companies", requestBody);

            // then
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Should_get_companies()
        {
            // given
            var httpClient = CreateHttpClient();
            var company = await AddCompany(httpClient, "Mengyao");

            // when
            var response = await httpClient.GetAsync("/companies");

            // then
            response.EnsureSuccessStatusCode();
            List<Company> companies = await DeserializeResponse<List<Company>>(response);
            Assert.Single(companies);
            Assert.Equal(company.Name, companies[0].Name);
        }

        [Fact]
        public async Task Should_get_existing_company()
        {
            // given
            var httpClient = CreateHttpClient();
            var company = await AddCompany(httpClient, "Mengyao");

            // when
            var response = await httpClient.GetAsync($"/companies/{company.Id}");

            // then
            response.EnsureSuccessStatusCode();
            Company gotCompany = await DeserializeResponse<Company>(response);
            Assert.Equal(gotCompany.Name, company.Name);
        }

        [Fact]
        public async Task Should_return_not_found_when_search_for_not_existing_company()
        {
            // given
            var httpClient = CreateHttpClient();

            // when
            var response = await httpClient.GetAsync($"/companies/hi");

            // then
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Should_get_companies_from_a_specific_page()
        {
            // given
            var httpClient = CreateHttpClient();
            var comp1 = await AddCompany(httpClient, "comp1");
            var comp2 = await AddCompany(httpClient, "comp2");
            var comp3 = await AddCompany(httpClient, "comp3");

            // when
            var response = await httpClient.GetAsync("/companies?pageSize=2&pageIndex=1");

            // then
            response.EnsureSuccessStatusCode();
            var companies = await DeserializeResponse<List<Company>>(response);
            Assert.Equal(2, companies.Count);
            Assert.Equal(comp1.Name, companies[0].Name);
            Assert.Equal(comp2.Name, companies[1].Name);
        }

        [Fact]
        public async Task Should_update_name_sucessfully()
        {
            // given
            var httpClient = CreateHttpClient();
            var comp1 = await AddCompany(httpClient, "comp1");
            comp1.Name = "NewComp1";
            var requestBody = CreateRequestBody(comp1);

            // when
            var response = await httpClient.PutAsync($"/companies/{comp1.Id}", requestBody);

            // then
            response.EnsureSuccessStatusCode();
            var company = await DeserializeResponse<Company>(response);
            Assert.Equal(comp1.Name, company.Name);
        }

        [Fact]
        public async Task Should_add_employee_for_a_company_sucessfully()
        {
            // given
            var httpClient = CreateHttpClient();
            var comp1 = await AddCompany(httpClient, "comp1");
            var employee = new Employee("YaoMeng", 1);
            var requestBody = CreateRequestBody(employee);

            // when
            var response = await httpClient.PostAsync($"/companies/{comp1.Id}/employees", requestBody);

            // then
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            Employee hiredEmployee = await DeserializeResponse<Employee>(response);
            Assert.Equal(employee.Name, hiredEmployee.Name);
            Assert.Equal(employee.Salary, hiredEmployee.Salary);
        }

        [Fact]
        public async Task Should_get_employees_for_a_company_sucessfully()
        {
            // given
            var httpClient = CreateHttpClient();
            var comp1 = await AddCompany(httpClient, "comp1");
            var employee = await AddEmployee(httpClient, "Yanxi", 1, comp1.Id);

            // when
            var response = await httpClient.GetAsync($"/companies/{comp1.Id}/employees");

            // then
            response.EnsureSuccessStatusCode();
            List<Employee> hiredEmployees = await DeserializeResponse<List<Employee>>(response);
            Assert.Single(hiredEmployees);
            Assert.Equal(employee.Name, hiredEmployees[0].Name);
            Assert.Equal(employee.Salary, hiredEmployees[0].Salary);
        }
    }
}

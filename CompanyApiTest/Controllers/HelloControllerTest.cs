using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class HelloControllerTest
    {
        public async Task<HttpClient> GetHttpClientAsync()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            _ = await client.DeleteAsync("companies/all");
            return client;
        }

        public void AddCompanyAndEmployee()
        {
        }

        [Fact]
        public async Task Should_create_company()
        {
            // given
            var httpClient = await GetHttpClientAsync();

            // when
            var response = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var responseString = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(responseString);

            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("slb", company.Name);
            Assert.NotNull(company.Id);
        }

        [Fact]
        public async Task Should_can_not_create_company_when_name_exist()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb"));

            // when
            var response = await httpClient.PostAsJsonAsync("companies", new Company("slb"));

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Should_get_all_companies()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb1"));

            // when
            var response = await httpClient.GetAsync("companies/all");
            var responseString = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseString);
            // then
            Assert.Equal(2, companies.Count);
        }

        [Fact]
        public async Task Should_get_company_by_id()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            var createResponse = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var company = JsonConvert.DeserializeObject<Company>(await createResponse.Content.ReadAsStringAsync());

            // when
            var response = await httpClient.GetAsync($"companies/{company.Id}");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseString);
            // then
            Assert.Equal(responseCompany, company);
        }

        [Fact]
        public async Task Should_get_X_page_companies()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb1"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb2"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb3"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb4"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb5"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb7"));
            _ = await httpClient.PostAsJsonAsync("companies", new Company("slb8"));

            // when
            var response = await httpClient.GetAsync("companies?pageSize=2&pageIndex=1");
            var responseString = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseString);
            // then
            Assert.Equal(2, companies.Count);
            Assert.Equal("slb", companies[0].Name);
            Assert.Equal("slb1", companies[1].Name);
        }

        [Fact]
        public async Task Should_udpate_company()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            var createResponse = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var company = JsonConvert.DeserializeObject<Company>(await createResponse.Content.ReadAsStringAsync());
            company.Name = "slb2";
            _ = await httpClient.PutAsJsonAsync($"companies/{company.Id}", company);
            // when
            var getResponse = await httpClient.GetAsync($"companies/{company.Id}");
            var responseString = await getResponse.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseString);
            // then
            Assert.Equal("slb2", responseCompany.Name);
        }

        [Fact]
        public async Task Should_add_an_employee_under_a_specific_company()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            var createResponse = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var company = JsonConvert.DeserializeObject<Company>(await createResponse.Content.ReadAsStringAsync());
            _ = await httpClient.PutAsJsonAsync($"companies/{company.Id}", company);

            // when
            var employee = new Employee("wxt");
            _ = await httpClient.PostAsJsonAsync($"companies/{company.Id}/employee/", employee);
            var getResponse = await httpClient.GetAsync($"companies/{company.Id}");
            var responseString = await getResponse.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseString);
            // then
            Assert.Single(responseCompany.EmployeeIDs);
            Assert.Equal(employee.EmployeeID, responseCompany.EmployeeIDs[0]);
        }

        [Fact]
        public async Task Should_get_all_employee_under_a_specific_company()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            var createResponse = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var company = JsonConvert.DeserializeObject<Company>(await createResponse.Content.ReadAsStringAsync());
            _ = await httpClient.PutAsJsonAsync($"companies/{company.Id}", company);
            _ = await httpClient.PostAsJsonAsync($"companies/{company.Id}/employee/", new Employee("wxt1"));
            _ = await httpClient.PostAsJsonAsync($"companies/{company.Id}/employee/", new Employee("wxt2"));

            var createResponse2 = await httpClient.PostAsJsonAsync("companies", new Company("slb2"));
            var company2 = JsonConvert.DeserializeObject<Company>(await createResponse2.Content.ReadAsStringAsync());
            _ = await httpClient.PutAsJsonAsync($"companies/{company2.Id}", company2);
            _ = await httpClient.PostAsJsonAsync($"companies/{company2.Id}/employee/", new Employee("wxt3"));
            _ = await httpClient.PostAsJsonAsync($"companies/{company2.Id}/employee/", new Employee("wxt4"));
            // when
            var getResponse = await httpClient.GetAsync($"companies/{company.Id}/all/employee");
            var responseString = await getResponse.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(responseString);
            // then
            Assert.Equal(2, employees.Count);
        }

        [Fact]
        public async Task Should_update_basic_information_of_a_specific_employee_under_a_specific_company()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            var createResponse = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var company = JsonConvert.DeserializeObject<Company>(await createResponse.Content.ReadAsStringAsync());
            _ = await httpClient.PutAsJsonAsync($"companies/{company.Id}", company);
            var employee = new Employee("wxt");
            _ = await httpClient.PostAsJsonAsync($"companies/{company.Id}/employee/", employee);

            employee.Name = "wxt2";
            _ = await httpClient.PutAsJsonAsync($"companies/{company.Id}/employee/{employee.EmployeeID}", employee);
            // when
            var getResponse = await httpClient.GetAsync($"companies/{company.Id}/all/employee");
            var responseString = await getResponse.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(responseString);
            // then
            Assert.Equal("wxt2", employees[0].Name);
        }

        [Fact]
        public async Task Should_delete_specific_employee_under_a_specific_company()
        {
            // given
            var httpClient = await GetHttpClientAsync();
            var createResponse = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var company = JsonConvert.DeserializeObject<Company>(await createResponse.Content.ReadAsStringAsync());
            _ = await httpClient.PutAsJsonAsync($"companies/{company.Id}", company);
            var employee = new Employee("wxt");
            _ = await httpClient.PostAsJsonAsync($"companies/{company.Id}/employee/", employee);
            _ = await httpClient.PostAsJsonAsync($"companies/{company.Id}/employee/", new Employee("wxt2"));

            _ = await httpClient.DeleteAsync($"companies/{company.Id}/employee/{employee.EmployeeID}");

            // when
            var getResponse = await httpClient.GetAsync($"companies/{company.Id}/all/employee");
            var responseString = await getResponse.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(responseString);
            // then
            Assert.Single(employees);
            Assert.Equal("wxt2", employees[0].Name);
        }
    }
}
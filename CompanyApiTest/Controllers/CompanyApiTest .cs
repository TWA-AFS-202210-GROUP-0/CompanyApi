using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace CompanyApiTest.Controllers
{
    public class CompanyApiTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var company = new Company(name: "SLB");
            // when
            var response = await httpClient.PostAsync("api/companies", SerializedObject(company));
            var createdCompany = await ParseObject<Company>(response);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyId);
        }

        [Fact]
        public async void Should_add_new_same_name_company_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var company = new Company("SLB");
            // when
            await httpClient.PostAsync("api/companies", SerializedObject(company));
            var response = await httpClient.PostAsync("api/companies", SerializedObject(company));
            var createdCompany = await ParseObject<Company>(response);
            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var companyOne = await CreateCompanyForTest(httpClient, "SLB");
            var companyTwo = await CreateCompanyForTest(httpClient, "Thoutworks");
            // when
            var response = await httpClient.GetAsync("api/companies");
            var companies = await ParseObject<List<Company>>(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, companies.Count);
            Assert.Equal(companyOne, companies[0]);
            Assert.Equal(companyTwo, companies[1]);
        }

        [Fact]
        public async void Should_get_specific_existing_company_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var companyOne = await CreateCompanyForTest(httpClient, "SLB");
            var companyTwo = CreateCompanyForTest(httpClient, "Thoutworks");
            // when
            var response = await httpClient.GetAsync($"api/companies/{companyOne.CompanyId}");
            var cmp = await ParseObject<Company>(response);
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(companyOne, cmp);
        }

        [Fact]
        public async void Should_get_index_X_company_to_index_Y_company_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            await CreateCompanyForTest(httpClient, "SLB");
            var companyTwo = await CreateCompanyForTest(httpClient, "Thoutworks");
            // when
            var response = await httpClient.GetAsync("api/companies?pageIndex=2&pageSize=1");
            var cmp = await ParseObject<List<Company>>(response);
           // then
           Assert.Equal(HttpStatusCode.OK, response.StatusCode);
           Assert.Equal(companyTwo, cmp[0]);
        }

        [Fact]
        public async void Should_update_basic_info_company_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();

            var companyOne = CreateCompanyForTest(httpClient, "SLB");
            var companyTwo = CreateCompanyForTest(httpClient, "Thoutworks");
            var companyOneMd = new Company(name: "Schlumberger");
            // when
            var response = await httpClient.PutAsync($"api/companies/{companyOne.Result.CompanyId}", SerializedObject(companyOneMd));
            var cmp = await ParseObject<Company>(response);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Schlumberger", cmp.Name);
        }

        [Fact]
        public async void Should_delete_one_company_and_its_employees_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();

            var companyOne = CreateCompanyForTest(httpClient, "SLB");
            var companyTwo = CreateCompanyForTest(httpClient, "Thoutworks");

            var personOne = await CreateEmployeeForTest(httpClient, "Ming", 2000, companyOne);
            var personTwo = await CreateEmployeeForTest(httpClient, "Hei", 10000, companyOne);

            // when
            await httpClient.DeleteAsync($"api/companies/{companyOne.Result.CompanyId}");
            var response = await httpClient.GetAsync("api/companies");
            var companies = ParseObject<List<Company>>(response);
            var responseEmp = await httpClient.GetAsync($"api/companies/{companyOne.Result.CompanyId}/employees");
            //then
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, responseEmp.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, companies.Result.Count);
        }

        [Fact]
        public async void Should_add_new_employee_for_a_company_given_company()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var companyOne = CreateCompanyForTest(httpClient, "SLB");
            var personOne = new Employee("Ming", 200);
            // when
            var response = await httpClient.PostAsync($"api/companies/{companyOne.Result.CompanyId}/employees", SerializedObject(personOne));
            var createdPerson = await ParseObject<Employee>(response);
            //then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(personOne, createdPerson);
            Assert.NotEmpty(createdPerson.EmployeeId);
        }

        [Fact]
        public async void Should_get_all_employees_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var companyOne = CreateCompanyForTest(httpClient, "SLB");
            var personOne = await CreateEmployeeForTest(httpClient, "Ming", 2000, companyOne);
            var personTwo = await CreateEmployeeForTest(httpClient, "Hei", 10000, companyOne);
            // when
            var response = await httpClient.GetAsync($"api/companies/{companyOne.Result.CompanyId}/employees");
            var employees = await ParseObject<List<Employee>>(response);
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, employees.Count);
            Assert.Equal(personOne, employees[0]);
            Assert.Equal(personTwo, employees[1]);
        }

        [Fact]
        public async void Should_update_basic_info_of_one_employee_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var companyOne = CreateCompanyForTest(httpClient, "SLB");
            var personOne = CreateEmployeeForTest(httpClient, "Ming", 2000, companyOne);
            var personOneMd = new Employee("Mwwwwg", 1500);
            var personTwo = CreateEmployeeForTest(httpClient, "Hei", 10000, companyOne);

            // when
            var response = await httpClient.PutAsync($"api/companies/{companyOne.Result.CompanyId}/employees/{personOne.Result.EmployeeId}",
                SerializedObject(personOneMd));
            var employee = await ParseObject<Employee>(response);
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(personOneMd, employee);
        }

        [Fact]
        public async void Should_delete_specific_employee_successfully()
        {
            // given
            var httpClient = await SetUpEnvironment();
            var companyOne = CreateCompanyForTest(httpClient, "SLB");
            var personOne = CreateEmployeeForTest(httpClient, "Ming", 2000, companyOne);
            var personTwo = CreateEmployeeForTest(httpClient, "Hei", 10000, companyOne);
            // when
            await httpClient.DeleteAsync($"api/companies/{companyOne.Result.CompanyId}/employees/{personOne.Result.EmployeeId}");
            var response = await httpClient.GetAsync($"api/companies/{companyOne.Result.CompanyId}/employees");
            var employees = await ParseObject<List<Employee>>(response);
            //Then
            Assert.Equal(1, employees.Count);
        }

        public async Task<HttpClient> SetUpEnvironment()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("api/companies");
            return httpClient;
        }

        public async Task<T> ParseObject<T>(HttpResponseMessage response)
       {
             var responseBody = await response.Content.ReadAsStringAsync();
             var createdObject = JsonConvert.DeserializeObject<T>(responseBody);
             return createdObject;
       }

        public StringContent SerializedObject(object obj)
       {
           var serializedObjectBody = JsonConvert.SerializeObject(obj);
           var postBodyOne = new StringContent(serializedObjectBody, Encoding.UTF8, "application/json");
           return postBodyOne;
       }

        public async Task<Company> CreateCompanyForTest(HttpClient httpClient, string companyName)
       {
           var companyOne = new Company(name: companyName);
           var postBodyOne = SerializedObject(companyOne);
           var response = await httpClient.PostAsync("api/companies", postBodyOne);
           var result = await ParseObject<Company>(response);
           return result;
       }

        public async Task<Employee> CreateEmployeeForTest(HttpClient httpClient, string personName, int personSalary, Task<Company> cmp)
       {
           var personOne = new Employee(personName, personSalary);
           var postBodyOne = SerializedObject(personOne);
           var company = cmp.Result;
           var response = await httpClient.PostAsync($"api/companies/{company.CompanyId}/employees", postBodyOne);
           var result = await ParseObject<Employee>(response);
           return result;
       }
    }
}

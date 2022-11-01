using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyApiTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var serializedCompany = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedCompany, Encoding.UTF8, "application/json");
            // when
            await httpClient.DeleteAsync("api/companies");
            var response = await httpClient.PostAsync("api/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyId);
        }

        [Fact]
        public async void Should_add_new_same_name_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var serializedCompany = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedCompany, Encoding.UTF8, "application/json");
            // when
            await httpClient.PostAsync("api/companies", postBody);
            var response = await httpClient.PostAsync("api/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var companyOne = new Company(name: "SLB");
            var companyTwo = new Company(name: "Thoughtworks");
            var serializedCompanyOne = JsonConvert.SerializeObject(companyOne);
            var postBodyOne = new StringContent(serializedCompanyOne, Encoding.UTF8, "application/json");
            var serializedCompanyTwo = JsonConvert.SerializeObject(companyTwo);
            var postBodyTwo = new StringContent(serializedCompanyTwo, Encoding.UTF8, "application/json");
            // when
            await httpClient.DeleteAsync("api/companies");
            await httpClient.PostAsync("api/companies", postBodyOne);
            await httpClient.PostAsync("api/companies", postBodyTwo);
            var response = await httpClient.GetAsync("api/companies");
            var responseBody = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, companies.Count);
            Assert.Equal(companyOne, companies[0]);
            Assert.Equal(companyTwo, companies[1]);
        }

        [Fact]
        public async void Should_get_specific_existing_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var companyOne = new Company(name: "SLB");
            var companyTwo = new Company(name: "Thoughtworks");
            var serializedCompanyOne = JsonConvert.SerializeObject(companyOne);
            var postBodyOne = new StringContent(serializedCompanyOne, Encoding.UTF8, "application/json");
            var serializedCompanyTwo = JsonConvert.SerializeObject(companyTwo);
            var postBodyTwo = new StringContent(serializedCompanyTwo, Encoding.UTF8, "application/json");
            // when
            await httpClient.DeleteAsync("api/companies");
            var responseGetSLB = await httpClient.PostAsync("api/companies", postBodyOne);
            await httpClient.PostAsync("api/companies", postBodyTwo);
            var responseSLBBody = await responseGetSLB.Content.ReadAsStringAsync();
            var cmpSLB = JsonConvert.DeserializeObject<Company>(responseSLBBody);
            var response = await httpClient.GetAsync($"api/companies/{cmpSLB.CompanyId}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var cmp = JsonConvert.DeserializeObject<Company>(responseBody);
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(companyOne, cmp);
        }

        [Fact]
        public async void Should_get_index_X_company_to_index_Y_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var companyOne = new Company(name: "SLB");
            var companyTwo = new Company(name: "Thoughtworks");
            var serializedCompanyOne = JsonConvert.SerializeObject(companyOne);
            var postBodyOne = new StringContent(serializedCompanyOne, Encoding.UTF8, "application/json");
            var serializedCompanyTwo = JsonConvert.SerializeObject(companyTwo);
            var postBodyTwo = new StringContent(serializedCompanyTwo, Encoding.UTF8, "application/json");
            // when
            await httpClient.DeleteAsync("api/companies");
            await httpClient.PostAsync("api/companies", postBodyOne);
            await httpClient.PostAsync("api/companies", postBodyTwo);
            var response = await httpClient.GetAsync("api/companies?pageIndex=2&pageSize=1");
            var responseBody = await response.Content.ReadAsStringAsync();
            var cmp = JsonConvert.DeserializeObject<List<Company>>(responseBody);
           // then
           Assert.Equal(HttpStatusCode.OK, response.StatusCode);
           Assert.Equal(companyTwo, cmp[0]);
        }

        [Fact]
        public async void Should_update_basic_info_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();

            var companyOne = new Company(name: "SLB");
            var companyOneMd = new Company(name: "Schlumberger");
            var companyTwo = new Company(name: "Thoughtworks");

            var serializedCompanyOne = JsonConvert.SerializeObject(companyOne);
            var postBodyOne = new StringContent(serializedCompanyOne, Encoding.UTF8, "application/json");

            var serializedCompanyOneMd = JsonConvert.SerializeObject(companyOneMd);
            var postBodyOneMd = new StringContent(serializedCompanyOneMd, Encoding.UTF8, "application/json");

            var serializedCompanyTwo = JsonConvert.SerializeObject(companyTwo);
            var postBodyTwo = new StringContent(serializedCompanyTwo, Encoding.UTF8, "application/json");
            // when
            await httpClient.DeleteAsync("api/companies");
            var responseGetSLB = await httpClient.PostAsync("api/companies", postBodyOne);
            await httpClient.PostAsync("api/companies", postBodyTwo);
            var responseSLBBody = await responseGetSLB.Content.ReadAsStringAsync();
            var cmpSLB = JsonConvert.DeserializeObject<Company>(responseSLBBody);
            var response = await httpClient.PutAsync($"api/companies/{cmpSLB.CompanyId}", postBodyOneMd);
            var responseBody = await response.Content.ReadAsStringAsync();
            var cmp = JsonConvert.DeserializeObject<Company>(responseBody);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Schlumberger", cmp.Name);
        }

        [Fact]
        public async void Should_add_new_employee_for_a_company_given_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var person = new Employee("Ming", 2000);

            var serializedCompany = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedCompany, Encoding.UTF8, "application/json");

            var serializedPerson = JsonConvert.SerializeObject(person);
            var postBodyPerson = new StringContent(serializedPerson, Encoding.UTF8, "application/json");
            // when
            await httpClient.DeleteAsync("api/companies");
            var responsePostSLB = await httpClient.PostAsync("api/companies", postBody);
            var responseSLBBody = await responsePostSLB.Content.ReadAsStringAsync();
            var cmpSLB = JsonConvert.DeserializeObject<Company>(responseSLBBody);
            var response = await httpClient.PostAsync($"api/companies/{cmpSLB.CompanyId}/employees", postBodyPerson);

            var responseBody = await response.Content.ReadAsStringAsync();
            var createdPerson = JsonConvert.DeserializeObject<Employee>(responseBody);
            //then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(person, createdPerson);
            Assert.NotEmpty(createdPerson.EmployeeId);
        }
    }
}

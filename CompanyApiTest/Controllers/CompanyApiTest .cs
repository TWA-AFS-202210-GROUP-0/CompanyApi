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
            await httpClient.DeleteAsync("api/deletAllCompanies");
            await httpClient.PostAsync("api/companies", postBodyOne);
            await httpClient.PostAsync("api/companies", postBodyTwo);
            var response = await httpClient.GetAsync("api/getAllCompanies");
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
            await httpClient.DeleteAsync("api/deletAllCompanies");
            await httpClient.PostAsync("api/companies", postBodyOne);
            await httpClient.PostAsync("api/companies", postBodyTwo);
            var response = await httpClient.GetAsync("api/getOneCmp?name=SLB");
            var responseBody = await response.Content.ReadAsStringAsync();
            var cmp = JsonConvert.DeserializeObject<Company>(responseBody);
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(companyOne, cmp);
        }
    }
}

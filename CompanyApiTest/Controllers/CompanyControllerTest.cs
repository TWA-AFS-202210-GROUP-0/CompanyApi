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
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
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
            var responseBody = await response.Content.ReadAsStringAsync();
            var companiesList = JsonConvert.DeserializeObject<List<Company>>(responseBody);
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
            var responseBody = await getResponse.Content.ReadAsStringAsync();
            var getCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", getCompany.Name);
        }

        public static StringContent BuildRequestBody(Company newCompany)
        {
            var companyJson = JsonConvert.SerializeObject(newCompany);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return postBody;
        }
    }
}

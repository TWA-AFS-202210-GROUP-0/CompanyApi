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
    }
}
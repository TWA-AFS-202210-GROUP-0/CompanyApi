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
    }
}
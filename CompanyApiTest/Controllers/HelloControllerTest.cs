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
        public HttpClient GetHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            return application.CreateClient();
        }

        [Fact]
        public async Task Should_return_hello_world_with_default_request()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();

            // when
            var response = await httpClient.GetAsync("/hello");
            var responseString = await response.Content.ReadAsStringAsync();

            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("Hello World", responseString);
        }

        [Fact]
        public async Task Should_create_company()
        {
            // given
            var httpClient = GetHttpClient();

            // when
            var response = await httpClient.PostAsJsonAsync("companies", new Company("slb"));
            var responseString = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(responseString);

            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("slb", company.Name);
            Assert.NotNull(company.Id);
        }
    }
}
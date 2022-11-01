using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_add_new_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var postBody = BuildRequestBody(company);
            // when
            var response = await httpClient.PostAsync("/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async Task Should_add_new_company_failly()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var postBody = BuildRequestBody(company);
            await httpClient.PostAsync("/companies", postBody);
            // when
            var response = await httpClient.PostAsync("/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Should_get_all_companies_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var slb = new Company(name: "SLB");
            var postBodySlb = BuildRequestBody(slb);
            await httpClient.PostAsync("/companies", postBodySlb);
            var tw = new Company(name: "TW");
            var postBodyTw = BuildRequestBody(tw);
            await httpClient.PostAsync("/companies", postBodyTw);
            // when
            var response = await httpClient.GetAsync("/companies");
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_get_company_by_id_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var slb = new Company(name: "SLB");
            var postBodySlb = BuildRequestBody(slb);
            await httpClient.PostAsync("/companies", postBodySlb);
            var tw = new Company(name: "TW");
            var postBodyTw = BuildRequestBody(tw);
            var responseForId = await httpClient.PostAsync("/companies", postBodyTw);
            var responseBodyExcepted = await responseForId.Content.ReadAsStringAsync();
            var responseExcepted = JsonConvert.DeserializeObject<Company>(responseBodyExcepted);
            string exceptedcompanyId = responseExcepted.CompanyID;
            // when
            var response = await httpClient.GetAsync($"/companies/{exceptedcompanyId}");
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseActual = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal("SLB", responseActual[0].Name);
        }

        private static StringContent BuildRequestBody(Company company)
        {
            company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return postBody;
        }
    }
}

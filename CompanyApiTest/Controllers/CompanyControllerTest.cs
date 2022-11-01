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
            var responseActual = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("TW", responseActual.Name);
        }

        [Fact]
        public async Task Should_get_all_companies_in_page_3_when_one_page_has_1_element()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            await httpClient.PostAsync("/companies", BuildRequestBody(new Company(name: "SLB")));
            await httpClient.PostAsync("/companies", BuildRequestBody(new Company(name: "TW")));
            await httpClient.PostAsync("/companies", BuildRequestBody(new Company(name: "IBM")));
            // when
            var response = await httpClient.GetAsync("/companies?pageSize=1&pageIndex=1");
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal("SLB", createdCompany[0].Name);
        }

        [Fact]
        public async void Should_update_company_information_of_an_existing_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var newCompany = new Company(name: "SLB");
            StringContent postBody = BuildRequestBody(newCompany);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            createdCompany.Name = "Schlumberger";
            StringContent updateBody = BuildRequestBody(createdCompany);
            //when
            var updateResponse = await httpClient.PutAsync($"/companies", updateBody);
            // then
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updatedBody = await updateResponse.Content.ReadAsStringAsync();
            var updatedCompany = JsonConvert.DeserializeObject<Company>(updatedBody);
            Assert.Equal("Schlumberger", updatedCompany.Name);
        }

        public static StringContent BuildRequestBody(Object newCompany)
        {
            var companyJson = JsonConvert.SerializeObject(newCompany);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return postBody;
        }
    }
}

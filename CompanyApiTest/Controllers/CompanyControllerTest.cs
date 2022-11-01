﻿using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        public HttpClient CreateHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            return application.CreateClient();
        }

        public StringContent CreateRequestBody(object obj)
        {
            var serializeObject = JsonConvert.SerializeObject(obj);
            return new StringContent(serializeObject, Encoding.UTF8, "application/json");
        }

        public async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        [Fact]
        public async Task Should_add_company_sucessfully()
        {
            // given
            var httpClient = CreateHttpClient();
            var company = new Company("Mengyao");
            var requestBody = CreateRequestBody(company);

            // when
            var response = await httpClient.PostAsync("/companies", requestBody);

            // then
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            Company createdCompany = await DeserializeResponse<Company>(response);
            Assert.Equal(company.Name, createdCompany.Name);
            Assert.NotEmpty(createdCompany.Id);
        }

        [Fact]
        public async Task Should_not_add_company_when_the_name_is_same()
        {
            // given
            var httpClient = CreateHttpClient();
            var company = new Company("Mengyao");
            var requestBody = CreateRequestBody(company);
            await httpClient.PostAsync("/companies", requestBody);

            // when
            var response = await httpClient.PostAsync("/companies", requestBody);

            // then
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Should_get_companies()
        {
            // given
            var httpClient = CreateHttpClient();
            var company = new Company("Mengyao");
            var requestBody = CreateRequestBody(company);
            await httpClient.PostAsync("/companies", requestBody);

            // when
            var response = await httpClient.GetAsync("/companies");

            // then
            response.EnsureSuccessStatusCode();
            List<Company> companies = await DeserializeResponse<List<Company>>(response);
            Assert.Single(companies);
            Assert.Equal(company.Name, companies[0].Name);
        }
    }
}

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Dto;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var newCompany = new CompanyDto()
            {
                Name = "SLB",
            };
            var createRequest = BuildRequest(newCompany);
            //When
            var createResponse = await httpClient.PostAsync("/companies", createRequest);
            //Then
            createResponse.EnsureSuccessStatusCode();
            var result = await ParseRespone<CompanyDto>(createResponse);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.Equal("SLB", result.Name);
        }

        [Fact]
        public async void Should_not_add_new_company_with_name_existed()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var newCompany = new CompanyDto()
            {
                Name = "SLB",
            };
            var createRequest = BuildRequest(newCompany);
            //When
            var createResponse = await httpClient.PostAsync("/companies", createRequest);
            var repeatCreateResponse = await httpClient.PostAsync("/companies", createRequest);
            //Then
            Assert.Equal(HttpStatusCode.Conflict, repeatCreateResponse.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            var getResponse = await httpClient.GetAsync("/companies");
            //Then
            getResponse.EnsureSuccessStatusCode();
            var result = await ParseRespone<List<CompanyDto>>(getResponse);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public async void Should_get_existing_company()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            //When
            var getResponse = await httpClient.GetAsync($"companies/{createdCompanyResult.CompanyId}");
            //Then
            getResponse.EnsureSuccessStatusCode();
            var getResult = await ParseRespone<CompanyDto>(getResponse);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal(createdCompanyResult.CompanyId, getResult.CompanyId);
        }

        [Fact]
        public async void Should_not_get_existing_company_given_wrong_ID()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            //When
            var getResponse = await httpClient.GetAsync("companies/wrongID");
            //Then
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async void Should_Get_2_by_pageSize3_pageIndex2_Given_5_items()
        {
            //Given
            var httpClient = SetUpEnviroment();
            for (int i = 0; i < 5; i++)
            {
                var newCompany = new CompanyDto()
                {
                    Name = $"SLB{i}",
                };
                var createRequest = BuildRequest(newCompany);

                var createResponse = await httpClient.PostAsync("/companies", createRequest);
                createResponse.EnsureSuccessStatusCode();
                var createResult = await ParseRespone<CompanyDto>(createResponse);
            }

            //When
            var getResponse = await httpClient.GetAsync("companies?pageSize=3&pageIndex=2");
            //Then
            getResponse.EnsureSuccessStatusCode();
            var getResult = await ParseRespone<List<Company>>(getResponse);
            Assert.Equal(2, getResult.Count);
        }

        [Fact]
        public async void Should_Get_2_by_pageSize2_pageIndex2_Given_5_items()
        {
            //Given
            var httpClient = SetUpEnviroment();
            for (int i = 0; i < 5; i++)
            {
                var newCompany = new CompanyDto()
                {
                    Name = $"SLB{i}",
                };
                var createRequest = BuildRequest(newCompany);

                var createResponse = await httpClient.PostAsync("/companies", createRequest);
                createResponse.EnsureSuccessStatusCode();
                var createResult = await ParseRespone<CompanyDto>(createResponse);
            }

            //When
            var getResponse = await httpClient.GetAsync("companies?pageSize=2&pageIndex=2");
            //Then
            getResponse.EnsureSuccessStatusCode();
            var getResult = await ParseRespone<List<CompanyDto>>(getResponse);
            Assert.Equal(2, getResult.Count);
        }

        [Fact]
        public async void Should_not_get_by_pageSize3_pageIndex3_Given_5_items()
        {
            //Given
            var httpClient = SetUpEnviroment();
            for (int i = 0; i < 5; i++)
            {
                var newCompany = new CompanyDto()
                {
                    Name = $"SLB{i}",
                };
                var createRequest = BuildRequest(newCompany);

                var createResponse = await httpClient.PostAsync("/companies", createRequest);
                createResponse.EnsureSuccessStatusCode();
                var createResult = await ParseRespone<CompanyDto>(createResponse);
            }

            //When
            var getResponse = await httpClient.GetAsync("companies?pageSize=3&pageIndex=3");
            //Then
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async void Should_Update_existing_company()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            //When
            var updateCompany = new CompanyDto()
            {
                Name = "SLBSLB",
            };
            var patchRequest = BuildRequest(updateCompany);
            var patchResponse = await httpClient.PatchAsync($"companies/{createdCompanyResult.CompanyId}", patchRequest);
            //Then
            patchResponse.EnsureSuccessStatusCode();
            var patchResult = await ParseRespone<CompanyDto>(patchResponse);
            Assert.Equal("SLBSLB", patchResult.Name);
        }

        [Fact]
        public async void Should_add_new_Employee()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            //When
            var newEmployee = new EmployeeDto()
            {
                Name = "Meng",
                Salary = 1000,
            };
            var createRequest = BuildRequest(newEmployee);
            var createResponse = await httpClient.PostAsync($"companies/{createdCompanyResult.CompanyId}/employees/", createRequest);
            //Then
            createResponse.EnsureSuccessStatusCode();
            var createEmployeeResult = await ParseRespone<EmployeeDto>(createResponse);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.Equal("Meng", createEmployeeResult.Name);
        }

        [Fact]
        public async void Should_get_employee_list()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            var meng = await CreateEmployeeForTest(httpClient, createdCompanyResult, "Meng");
            var yanxi = await CreateEmployeeForTest(httpClient, createdCompanyResult, "Yanxi");
            //When
            //When
            var getResponse = await httpClient.GetAsync($"companies/{createdCompanyResult.CompanyId}/employees");
            //Then
            getResponse.EnsureSuccessStatusCode();
            var getResult = await ParseRespone<List<EmployeeDto>>(getResponse);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal(2, getResult.Count);
        }

        [Fact]
        public async void Should_update_employee()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            var meng = await CreateEmployeeForTest(httpClient, createdCompanyResult, "Meng");
            var updateEmployee = new EmployeeDto()
            {
                Name = "Meng Yao",
                Salary = 2000,
            };
            var patchRequest = BuildRequest(updateEmployee);
            var patchResponse = await httpClient.PatchAsync($"companies/{createdCompanyResult.CompanyId}/employees/{meng.EmployeeId}", patchRequest);
            //Then
            patchResponse.EnsureSuccessStatusCode();
            var patchResult = await ParseRespone<EmployeeDto>(patchResponse);
            Assert.Equal("Meng Yao", patchResult.Name);
            Assert.Equal(2000, patchResult.Salary);
        }

        [Fact]
        public async void Should_remove_employee()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            var meng = await CreateEmployeeForTest(httpClient, createdCompanyResult, "Meng");
            var deleteResponse = await httpClient.DeleteAsync($"companies/{createdCompanyResult.CompanyId}/employees/{meng.EmployeeId}");
            var getResponse =
                await httpClient.GetAsync($"companies/{createdCompanyResult.CompanyId}/employees/{meng.EmployeeId}");
            //Then
            deleteResponse.EnsureSuccessStatusCode();
            var responeAsStringAsync = await deleteResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            Assert.Equal("Employee removed", responeAsStringAsync);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async void Should_remove_company()
        {
            //Given
            var httpClient = SetUpEnviroment();
            var createdCompanyResult = await CreateCompanyForTest(httpClient);
            var meng = await CreateEmployeeForTest(httpClient, createdCompanyResult, "Meng");
            var deleteResponse = await httpClient.DeleteAsync($"companies/{createdCompanyResult.CompanyId}");
            //Then
            deleteResponse.EnsureSuccessStatusCode();
            var responeAsStringAsync = await deleteResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            Assert.Equal("Company removed", responeAsStringAsync);
        }

        public HttpClient SetUpEnviroment()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            httpClient.DeleteAsync("/companies");
            return httpClient;
        }

        public StringContent BuildRequest(object obj)
        {
            var requestJson = JsonConvert.SerializeObject(obj);
            return new StringContent(requestJson, Encoding.UTF8, "application/json");
        }

        public async Task<T> ParseRespone<T>(HttpResponseMessage response)
        {
            var responeAsStringAsync = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responeAsStringAsync);
        }

        private async Task<CompanyDto> CreateCompanyForTest(HttpClient httpClient)
        {
            var newCompany = new CompanyDto()
            {
                Name = "SLB",
            };
            var createRequest = BuildRequest(newCompany);
            //When
            var createResponse = await httpClient.PostAsync("/companies", createRequest);
            createResponse.EnsureSuccessStatusCode();
            var result = await ParseRespone<CompanyDto>(createResponse);
            return result;
        }

        private async Task<EmployeeDto> CreateEmployeeForTest(HttpClient httpClient, CompanyDto createdCompanyResult, string name)
        {
            //When
            var newEmployee = new EmployeeDto()
            {
                Name = name,
                Salary = 1000,
            };
            var createRequest = BuildRequest(newEmployee);
            var createResponse =
                await httpClient.PostAsync($"companies/{createdCompanyResult.CompanyId}/employees/", createRequest);
            var createEmployeeResult = await ParseRespone<EmployeeDto>(createResponse);
            return createEmployeeResult;
        }
    }
}

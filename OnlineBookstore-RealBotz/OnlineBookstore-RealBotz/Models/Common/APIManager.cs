using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Model.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnlineBookstore_RealBotz.Models.Common
{
    public class APIManager
    {
        private HttpClient _httpClient;
        private readonly string ApiBaseUrl;
        private readonly string username;
        private readonly string password;

        public APIManager(IConfiguration configuration)
        {
            ApiBaseUrl = configuration["ApiBaseUrl"];
            username = configuration["Uname"];
            password = configuration["Password"];
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiBaseUrl);
        }

        public async Task<string> CallPostMethod(string endpoint, object requestData, string jwtToken)
        {                        
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // Serialize the requestData object to JSON
            string jsonRequest = JsonConvert.SerializeObject(requestData);

            // Create a StringContent from the JSON data
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            // Send a POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync(ApiBaseUrl + endpoint, content);

            string responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
        public async Task<string> GenerateToken()
        {
            var request = new LoginModel();
            request.Email = username;
            request.Password = password;

            string jsonRequest = JsonConvert.SerializeObject(request);

            // Create a StringContent from the JSON data
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            // Send a POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync(ApiBaseUrl + "Account/authentication", content);

            return await response.Content.ReadAsStringAsync();

        }
    }
}

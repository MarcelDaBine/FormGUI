using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SquadGUI.Services;

public class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly string _url;

    public HttpService()
    {
        _httpClient = new HttpClient();
        _url = "http://localhost:8090/api/v1/auth/authenticate";
    }

    public async Task<bool> Authenticate(string email, string password)
    {
        try
        {
            var credentials = new
            {
                email = email,
                password = password
            };
            
            string jsonData = JsonSerializer.Serialize(credentials);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_url, content);

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
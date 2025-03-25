using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using SquadGUI.Assets.Templates;
using SquadGUI.Interfaces;

namespace SquadGUI.Services;

// Class responsible for handling HTTP requests and authentication
public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    private string? _jwtToken;

    // Constructor initializes HttpClient and sets the authentication URL
    public HttpService()
    {
        _httpClient = new HttpClient();
        _url = "http://localhost:8090/api/v1/";
    }

    // Method to authenticate user with email and password
    /// <summary>
    /// Authenticates a user with their email and password.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="password">The user's password</param>
    /// <returns>A string containing the authentication response or error message</returns>
    public async Task<string> Authenticate(string email, string password)
    {
        try
        {
            var credentials = new { email, password };
            string jsonData = JsonSerializer.Serialize(credentials);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_url + "auth/authenticate", content);
            
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    if (cookie.StartsWith("jwt="))
                    {
                        _jwtToken = cookie.Split(';')[0].Split('=')[1];
                        Console.WriteLine($"Stored JWT: {_jwtToken}");
                        break;
                    }
                }
            }

            var result = JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync());
            return result?.Code switch
            {
                "SUCCESS" => "Success",
                "WRONG_CREDENTIALS" => "Wrong Credentials, please try again",
                "UNDEFINED_ERROR" => "Something went wrong, please try again",
                "CREDENTIALS_IN_USE" => "Credential already in use, please try again",
                _ => $"Unknown response: {result?.Code ?? "null"}"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return "Something went wrong, please try again";
        }
    }
    

    public async Task<string> SaveAsync(string data)
    {
        return await PostJsonWithJwt($"{_url}form/save", data);
    }

    public async Task<string> SubmitAsync(string data)
    {
        return await PostJsonWithJwt($"{_url}form/submit", data);
    }
    
    private async Task<string> PostJsonWithJwt(string url, string data)
    {
        /*
        if (string.IsNullOrWhiteSpace(_jwtToken))
            return "Not authenticated. Please log in.";
        */
        try
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            
            request.Headers.Add("Cookie", $"jwt={_jwtToken}");
            Console.WriteLine($"[Debug] Sent Cookie: jwt={_jwtToken}");

            var response = await _httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return "Operation successful";
            else
                return $"Failed: {response.StatusCode} - {responseBody}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
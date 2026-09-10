using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormGUI.Services;

// Class responsible for handling HTTP requests and authentication
public class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly string _url;

    // Constructor initializes HttpClient and sets the authentication URL
    public HttpService()
    {
        _httpClient = new HttpClient();
        _url = "http://localhost:8090/api/v1/auth/authenticate";
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
            var credentials = new
            {
                email = email,
                password = password
            };
            
            string jsonData = JsonSerializer.Serialize(credentials);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_url, content);
            
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseBody}");
            
            return ResponseToString(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return "Something went wrong, please try again";
        }
        
    }
    
    // Helper method to convert API response to user-friendly message
    private string ResponseToString(string response)
    {
        switch (response.Substring(1, response.Length - 2))
        {
            case "SUCCESS":
                return "Success";
                
            case "WRONG_CREDENTIALS":
                return "Wrong Credentials, please try again";
                
            case "UNDEFINED_ERROR":
                return "Something went wrong, please try again";
            
            case "CREDENTIALS_IN_USE":
                return "Credential already in use, please try again";
        }
        return "AaAaAaAaAaAa...";
    }
}
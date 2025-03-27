using System.Text.Json.Serialization;

namespace SquadGUI.Assets.Templates;

public class ApiResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class ApiResponse<T>
{
    public string Code { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}


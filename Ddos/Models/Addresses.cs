using System.Text.Json.Serialization;

namespace Ddos.Models;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class Http
{
    [JsonPropertyName("address")]
    public string Address { get; set; }
}

public class Socket
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class Addresses
{
    [JsonPropertyName("http")]
    public List<Http> Http { get; set; }

    [JsonPropertyName("sockets")]
    public List<Socket> Sockets { get; set; }
}


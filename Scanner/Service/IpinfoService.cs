using System.Text.RegularExpressions;
using Scanner.Reports.Types;

namespace Scanner.Service;

public partial class IpinfoService
{
    private readonly HttpClient _client;
    private readonly ILogger<IpinfoService> _logger;

    public IpinfoService(ILogger<IpinfoService> logger)
    {
        _logger = logger;
        _client = new HttpClient();
    }

    /// <summary>
    /// Obtains information for a given IP address, using the ipinfo.io API
    /// </summary>
    /// <param name="ip">The IP address to get information for</param>
    public async Task<Address> GetIpInfo(string ip)
    {
        _client.DefaultRequestHeaders.Add("User-Agent", "ariel AT costas.dev - Domain Scanner v0.1");
        _logger.LogInformation("Fetching IP info for {0}", ip);
        var response = await _client.GetAsync($"https://ipinfo.io/{ip}/json");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch IP info for {ip}. Failed with status code {response.StatusCode}");
        }

        IpinfoResponse? content;
        try
        {
            content = await response.Content.ReadFromJsonAsync<IpinfoResponse>();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to parse IP info. Exception: {e.Message}" +
                                $"Response body: " + await response.Content.ReadAsStringAsync());
        }

        if (content == null)
        {
            throw new Exception("Failed to parse IP info. Response body not parsed");
        }

        var separateAsFromOrg = SeparateAsnFromOrg();
        var match = separateAsFromOrg.Match(content.Org);

        return new Address
        {
            Value = ip,
            ReverseName = content.Hostname ?? string.Empty,
            Asn = match.Groups["asn"].Value,
            OrgName = match.Groups["org"].Value,
            City = content.City,
            Region = content.Region,
            Country = content.Country
        };
    }

    [GeneratedRegex(@"(?<asn>AS\d+) (?<org>.*)$")]
    private static partial Regex SeparateAsnFromOrg();
}

// ReSharper disable once ClassNeverInstantiated.Global - This class is instantiated by deserialization
public class IpinfoResponse
{
    public string Hostname { get; set; } = string.Empty;
    public required string Org { get; set; }

    public required string City { get; set; }
    public required string Region { get; set; }
    public required string Country { get; set; }
}
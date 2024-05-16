using System.Text.RegularExpressions;
using Scanner.Reports.Types;

namespace Scanner.Reports.Fetchers;

public partial class IpinfoFetcher
{
    private readonly HttpClient _client;
    public IpinfoFetcher()
    {
        _client = new HttpClient();
    }
    
    /// <summary>
    /// Obtains information for a given IP address, using the ipinfo.io API
    /// </summary>
    /// <param name="ip">The IP address to get information for</param>
    public async Task<Address> GetIpInfo(string ip)
    {
        _client.DefaultRequestHeaders.Add("User-Agent", "ariel AT costas.dev - Domain Scanner v0.1");
        var response = await _client.GetAsync($"https://ipinfo.io/{ip}/json");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch IP info for {ip}");
        }
        
        var content = await response.Content.ReadFromJsonAsync<IpinfoResponse>();
        if (content == null)
        {
            throw new Exception($"Failed to parse IP info for {ip}");
        }
        
        var separateAsFromOrg = SeparateAsnFromOrg();
        var match = separateAsFromOrg.Match(content.Org);
        
        return new Address
        {
            Value = ip,
            ReverseName = content.Hostname,
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
    public required string Hostname { get; set; }
    public required string Org { get; set; }
    
    public required string City { get; set; }
    public required string Region { get; set; }
    public required string Country { get; set; }
}
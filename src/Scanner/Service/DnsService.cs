using DnsClient;

namespace Scanner.Service;

public class DnsService
{
    private readonly LookupClient _client;

    public DnsService()
    {
        var options = new LookupClientOptions()
        {
            ThrowDnsErrors = true
        };
        _client = new LookupClient(options);
    }
    
    /// <summary>
    /// Validates a domain exists and is reachable.
    /// </summary>
    public bool PrevalidateDomain(string domain)
    {
        try
        {
            _client.Query(domain, QueryType.ANY);
            return true;
        }
        catch (DnsResponseException e)
        {
            return e.Code != DnsResponseCode.NotExistentDomain;
        }
    }
    
    public string[] GetNameServers(string domain)
    {
        return GetRecords(domain, QueryType.NS);
    }

    public IEnumerable<string> GetApexAddresses(string domain)
    {
        var ip4 = GetRecords(domain, QueryType.A);
        var ip6 = GetRecords(domain, QueryType.AAAA);
        return ip4.Concat(ip6);
    }
    
    public string[] GetApexTextRecords(string domain)
    {
        return GetRecords(domain, QueryType.TXT);
    }
    
    public string[] GetMxRecords(string domain)
    {
        return GetRecords(domain, QueryType.MX);
    }
    
    private string[] GetRecords(string domain, QueryType queryType)
    {
        try
        {
            var result = _client.Query(domain, queryType);
            return queryType switch
            {
                QueryType.NS => result.Answers.NsRecords().Select(r => r.NSDName.Value).ToArray(),
                QueryType.A => result.Answers.ARecords().Select(r => r.Address.ToString()).ToArray(),
                QueryType.AAAA => result.Answers.AaaaRecords().Select(r => r.Address.ToString()).ToArray(),
                QueryType.MX => result.Answers.MxRecords().Select(r => r.Exchange.Value).ToArray(),
                QueryType.TXT => result.Answers.TxtRecords().SelectMany(r => r.Text).ToArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(queryType), queryType, null)
            };
        }
        catch (DnsResponseException e)
        {
            throw new Exception($"Code: {e.Code}, DnsError: {e.DnsError}");
        }
    }
}
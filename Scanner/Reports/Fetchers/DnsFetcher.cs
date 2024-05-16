using DnsClient;

namespace Scanner.Reports.Fetchers;

public class DnsFetcher
{
    private readonly LookupClient _client;

    public DnsFetcher()
    {
        var options = new LookupClientOptions()
        {
            ThrowDnsErrors = true
        };
        _client = new LookupClient(options);
    }
    
    /// <summary>
    /// Returns the name servers for a given domain
    ///
    /// For example: GetNameServers("google.com") will return:
    /// - ns1.google.com
    /// - ns2.google.com
    /// - ns3.google.com
    /// - ns4.google.com 
    /// </summary>
    /// <param name="domain">The domain to get the name servers for</param>
    public string[] GetNameServers(string domain)
    {
        try
        {
            var result = _client.Query(domain, QueryType.NS);
            return result.Answers.NsRecords().Select(x => x.NSDName.Value).ToArray();
        }
        catch (DnsResponseException e)
        {
            if (e.Code == DnsResponseCode.NotExistentDomain)
            {
                
            }
            throw new Exception($"Code: {e.Code}, DnsError: {e.DnsError}");
        }
    }

    public string[] GetApexAddresses(string domain)
    {
        try
        {
            var ip4 = _client.Query(domain, QueryType.A);
            var ip6 = _client.Query(domain, QueryType.AAAA);
            
            var ips = new List<string>();
            ips.AddRange(ip4.Answers.ARecords().Select(x => x.Address.ToString()));
            ips.AddRange(ip6.Answers.AaaaRecords().Select(x => x.Address.ToString()));
            
            return ips.ToArray();
        }
        catch (DnsResponseException e)
        {
            throw new Exception($"Code: {e.Code}, DnsError: {e.DnsError}");
        }
    }
    
    public string[] GetApexTextRecords(string domain)
    {
        try
        {
            var result = _client.Query(domain, QueryType.TXT);
            return result.Answers.TxtRecords().SelectMany(r => r.Text).ToArray();
        }
        catch (DnsResponseException e)
        {
            throw new Exception($"Code: {e.Code}, DnsError: {e.DnsError}");
        }
    }
}
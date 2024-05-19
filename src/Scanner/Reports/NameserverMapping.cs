using System.Text.RegularExpressions;
using Scanner.Reports.Types;

namespace Scanner.Reports;

public static partial class NameserverLookupService
{
    private static List<EndwithNameserverMapping> EndwithMappings { get; set; }
    private static List<PatternNameserverMapping> PatternMappings { get; set; }

    static NameserverLookupService()
    {
        EndwithMappings = new List<EndwithNameserverMapping>
        {
            new(".dondominio.com.", NameserverService.DonDominio),
            new(".dinahosting.com.", NameserverService.DinaHosting),
            new(".ns.cloudflare.com.", NameserverService.Cloudflare),
            new(".nsone.net", NameserverService.IbmNsone)
        };

        PatternMappings = new List<PatternNameserverMapping>
        {
            new(AzureDnsRegex(), NameserverService.Azure),
            new(IonosRegex(), NameserverService.Ionos)
        };
    }

    public static NameserverService GetService(string nameserver)
    {
        var endMatch = EndwithMappings.FirstOrDefault(m => nameserver.EndsWith(m.End));
        if (endMatch != null)
        {
            return endMatch.Service;
        }

        var patternMatch = PatternMappings.FirstOrDefault(m => m.Pattern.IsMatch(nameserver));
        return patternMatch?.Service ?? NameserverService.Unknown;
    }

    [GeneratedRegex(@"^ns\d+-\d+\.azure-dns\.(?:com|net|info|org)\.?$")]
    private static partial Regex AzureDnsRegex();

    [GeneratedRegex(@"ns-\w+.1and1-dns\.(?:com|org|net|eu|de|es|biz)\.?$")]
    private static partial Regex IonosRegex();
}

internal class EndwithNameserverMapping(string end, NameserverService service)
{
    public string End { get; } = end;
    public NameserverService Service { get; } = service;
}

internal class PatternNameserverMapping(Regex pattern, NameserverService service)
{
    public Regex Pattern { get; } = pattern;
    public NameserverService Service { get; } = service;
}

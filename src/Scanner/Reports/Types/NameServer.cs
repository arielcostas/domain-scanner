namespace Scanner.Reports.Types;

public class NameServer
{
    public required string Hostname { get; set; }
    public required NameserverService Service { get; set; }
}

public enum NameserverService : uint
{
    Unknown = 0,
    DonDominio = 1,
    Cloudflare = 2,
    Azure = 3,
    DinaHosting = 4,
    Ionos = 5,
    IbmNsone = 6,
    Akamai = 7
}
namespace Scanner.Reports.Types;

public class Address
{
    public required string Value { get; set; }
    public required string ReverseName { get; set; }
    public required string Asn { get; set; }
    
    public required string OrgName { get; set; }
    public required string City { get; set; }
    public required string Region { get; set; }
    public required string Country { get; set; }
}
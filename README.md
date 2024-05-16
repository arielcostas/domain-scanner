# Domain scanner

A small tool that scans a domain's main A/AAAA records, TXT records and nameservers where the domain is hosted.

This tool was built to apply my knowledge of Microsoft Azure, using CosmosDB for the database.

## Configuration

The configuration is done through the usual `appsettings.<environment>.json` files, or through environment variables or
[user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0).

```json
{
  "Cosmos": {
    "Endpoint": "https://<cosmosdb-account>.documents.azure.com:443/",
    "Key": "<cosmosdb-key>",
    "Database": "<cosmosdb-database>"
  }
}
```

## Detailed documentation

The detailed documentation can be found in the [GitHub Wiki](https://github.com/arielcostas/domain-scanner/wiki).

## License

This project is licensed under the MIT Licence - see the [LICENCE](LICENCE) file for details.
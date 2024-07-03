using Scanner;
using Scanner.Reports;
using Scanner.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IpinfoService>();
builder.Services.AddSingleton<DnsService>();
builder.Services.AddSingleton<IReportService, CosmosReportService>();

await builder.SetupCosmosDb();

builder.Services.AddHostedService<ReportProcessor>();

var app = builder.Build();
    
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
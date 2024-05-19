using Scanner;
using Scanner.Reports;
using Scanner.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IpinfoService>();
builder.Services.AddSingleton<DnsService>();
builder.Services.AddSingleton<IReportService, CosmosReportService>();

await builder.SetupCosmosDb();

builder.Services.AddHostedService<ReportProcessor>();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
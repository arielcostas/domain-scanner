using Scanner;
using Scanner.Reports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
await builder.SetupCosmosDb();

builder.Services.AddHostedService<ReportProcessor>();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute("default", "{controller=Report}/{action=List}/{id?}");

app.Run();
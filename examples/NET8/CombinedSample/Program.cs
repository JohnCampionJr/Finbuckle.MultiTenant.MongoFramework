using CombinedSample;
using DataIsolationSample.Data;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();

services.AddRouting(o => o.LowercaseUrls = true);

services.AddMultiTenant<MongoTenantInfo>()
    .WithMongoFrameworkStore(builder.Configuration.GetConnectionString("TenantStoreConnection"))
    .WithRouteStrategy()
    .WithRedirectStrategy("/notenant/index");

services.AddMongoPerTenantConnection(builder.Configuration.GetConnectionString("DefaultPerTenantConnection"));

// Register the db context, but do not specify a provider/connection string since
// these vary by tenant.
services.AddMongoDbContext<ToDoDbContext>();

var app = builder.Build();

await SeedService.Seed(app);

if (app.Environment.EnvironmentName == "Development")
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseMultiTenant();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{__tenant__}/{controller=Home}/{action=Index}");
    endpoints.MapControllerRoute("notenant", "/{controller=NoTenant}/{action=Index}/{id?}");
});

await app.RunAsync();

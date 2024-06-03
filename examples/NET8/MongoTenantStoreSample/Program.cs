using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoTenantStoreSample;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();

services.AddMultiTenant<MongoTenantInfo>()
        .WithMongoFrameworkStore(builder.Configuration.GetConnectionString("TenantStoreConnection"))
        .WithRouteStrategy();

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
    endpoints.MapControllerRoute("default", "{__tenant__=}/{controller=Home}/{action=Index}");
});

await app.RunAsync();

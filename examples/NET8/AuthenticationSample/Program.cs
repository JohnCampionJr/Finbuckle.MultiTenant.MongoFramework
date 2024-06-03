using System.Collections.Generic;
using AuthenticationSample;
using AuthenticationSample.Data;
using Finbuckle.Utilities.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Register per tenant connection to be used in multi tenant context
services.AddMongoPerTenantConnection(builder.Configuration.GetConnectionString("DefaultPerTenantConnection"));

// Register the db context, but do not specify a provider/connection
// string since these vary by tenant.
services.AddMongoDbContext<ApplicationDbContext>();

services.AddDefaultIdentity<MongoTenantIdentityUser>()
        .AddMongoFrameworkStores<ApplicationDbContext>();

// these wire up Finbuckle to work with pages correctly using the route strategy. If another strategy (host) is used, these would not be necessary
services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.Add(new MultiTenantPageRouteModelConvention());
});
services.DecorateService<LinkGenerator, AmbientValueLinkGenerator>(new List<string> { "__tenant__" });

services.AddMultiTenant<SampleTenantInfo>()
        .WithRouteStrategy()
        .WithRedirectStrategy("/notenant/notenant")
        .WithMongoFrameworkStore(builder.Configuration.GetConnectionString("TenantStoreConnection"))
        .WithPerTenantAuthentication();

var app = builder.Build();

await SeedService.Seed(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseMultiTenant();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

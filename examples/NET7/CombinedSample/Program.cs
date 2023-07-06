// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CombinedSample;
using DataIsolationSample.Data;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

//NOTE: this service will be called by the runtime when application is actually started
services.AddSingleton<IHostedService, ApplicationStartedService>();


var app = builder.Build();

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

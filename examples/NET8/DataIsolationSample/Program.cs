// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using DataIsolationSample;
using DataIsolationSample.Data;
using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoFramework;
using MongoTenantStoreSample;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllersWithViews();

services.AddMultiTenant<TenantInfo>()
    .WithConfigurationStore()
    .WithRouteStrategy()
    .WithRedirectStrategy("/notenant/index");

services.AddScoped<IMongoPerTenantConnection, MongoPerTenantConnection>();

// Register the db context, but do not specify a provider/connection string since
// these vary by tenant.
services.AddMongoDbContext<ToDoDbContext>();

var app = builder.Build();

await SeedService.Seed();

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

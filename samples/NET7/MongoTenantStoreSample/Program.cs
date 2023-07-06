// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoTenantStoreSample;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();

services.AddMultiTenant<MongoTenantInfo>()
        .WithMongoFrameworkStore(builder.Configuration.GetConnectionString("TenantStoreConnection"))
        .WithRouteStrategy();

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
    endpoints.MapControllerRoute("default", "{__tenant__=}/{controller=Home}/{action=Index}");
});

app.RunAsync();

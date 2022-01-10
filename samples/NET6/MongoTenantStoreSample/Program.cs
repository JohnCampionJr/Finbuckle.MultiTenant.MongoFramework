// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();

services.AddMultiTenant<MongoTenantInfo>()
        .WithMongoFrameworkStore(builder.Configuration.GetConnectionString("TenantStoreConnection"))
        .WithRouteStrategy();

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

// Seed the database the multitenant store will need.
var provider = builder.Services.BuildServiceProvider();
SetupStore(provider);


static void SetupStore(IServiceProvider sp)
{
    var scopeServices = sp.CreateScope().ServiceProvider;
    var store = scopeServices.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();

    if (store.GetAllAsync().Result.Any()) return;

    store.TryAddAsync(new MongoTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle", ConnectionString = "finbuckle_conn_string" }).Wait();
    store.TryAddAsync(new MongoTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "initech_conn_string" }).Wait();
    store.TryAddAsync(new MongoTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc", ConnectionString = "megacorp_conn_string" }).Wait();
}

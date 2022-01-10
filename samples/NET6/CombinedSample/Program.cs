// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using DataIsolationSample.Data;
using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();

services.AddMultiTenant<MongoTenantInfo>()
    .WithMongoFrameworkStore(builder.Configuration.GetConnectionString("TenantStoreConnection"))
    .WithRouteStrategy()
    .WithRedirectStrategy("/notenant/index");

services.AddMongoPerTenantConnection(builder.Configuration.GetConnectionString("DefaultPerTenantConnection"));

// Register the db context, but do not specify a provider/connection string since
// these vary by tenant.
services.AddMongoDbContext<ToDoDbContext>();

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

// Seed the database the multitenant store will need.
var provider = builder.Services.BuildServiceProvider();
SetupStore(provider);
SetupDb(provider, app.Configuration);


static void SetupStore(IServiceProvider sp)
{
    var scopeServices = sp.CreateScope().ServiceProvider;
    var store = scopeServices.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();

    if (store.GetAllAsync().Result.Any()) return;

    store.TryAddAsync(new MongoTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle" }).Wait();
    store.TryAddAsync(new MongoTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-tenant-initech" }).Wait();
    store.TryAddAsync(new MongoTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc" }).Wait();
}

void SetupDb(IServiceProvider sp, IConfiguration config)
{
    var scopeServices = sp.CreateScope().ServiceProvider;
    var store = scopeServices.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();

    var ti = store.TryGetByIdentifierAsync("finbuckle").Result;
    if (ti.ConnectionString is null)
        ti.ConnectionString = config.GetConnectionString("DefaultPerTenantConnection");

    var conn = new MongoPerTenantConnection(ti);
    using var db1 = new ToDoDbContext(conn, ti);
    if (!db1.ToDoItems.Any())
    {
        db1.ToDoItems.Add(new ToDoItem { Title = "Call Lawyer ", Completed = false });
        db1.ToDoItems.Add(new ToDoItem { Title = "File Papers", Completed = false });
        db1.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
        db1.SaveChanges();
    }

    ti = store.TryGetByIdentifierAsync("megacorp").Result;
    if (ti.ConnectionString is null) ti.ConnectionString = config.GetConnectionString("DefaultPerTenantConnection");
    conn = new MongoPerTenantConnection(ti);
    using var db2 = new ToDoDbContext(conn, ti);
    if (!db2.ToDoItems.Any())
    {
        db2.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
        db2.ToDoItems.Add(new ToDoItem { Title = "Construct Additional Pylons", Completed = true });
        db2.ToDoItems.Add(new ToDoItem { Title = "Call Insurance Company", Completed = false });
        db2.SaveChanges();
    }

    ti = store.TryGetByIdentifierAsync("initech").Result;
    if (ti.ConnectionString is null) ti.ConnectionString = config.GetConnectionString("DefaultPerTenantConnection");
    conn = new MongoPerTenantConnection(ti);
    using var db3 = new ToDoDbContext(conn, ti);
    if (!db3.ToDoItems.Any())
    {
        db3.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = false });
        db3.ToDoItems.Add(new ToDoItem { Title = "Pay Salaries", Completed = true });
        db3.ToDoItems.Add(new ToDoItem { Title = "Write Memo", Completed = false });
        db3.SaveChanges();
    }
}

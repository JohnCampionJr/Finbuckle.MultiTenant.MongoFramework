// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using DataIsolationSample.Data;
using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;


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

SetupDb();

void SetupDb()
{

    var ti = new TenantInfo { Id = "tenant-finbuckle-d043favoiaw", ConnectionString = "mongodb://localhost/isolation-test", Identifier = "finbuckle" };
    var conn = new MongoPerTenantConnection(ti);
    using (var db = new ToDoDbContext(conn, ti))
    {
        if (!db.ToDoItems.Any())
        {
            db.ToDoItems.Add(new ToDoItem { Title = "Call Lawyer ", Completed = false });
            db.ToDoItems.Add(new ToDoItem { Title = "File Papers", Completed = false });
            db.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
            db.SaveChanges();
        }
    }
    ti = new TenantInfo { Id = "tenant-megacorp-g754dafg", ConnectionString = "mongodb://localhost/isolation-test", Identifier = "megacorp" };
    conn = new MongoPerTenantConnection(ti);
    using var db1 = new ToDoDbContext(conn, ti);
    if (!db1.ToDoItems.Any())
    {
        db1.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
        db1.ToDoItems.Add(new ToDoItem { Title = "Construct Additional Pylons", Completed = true });
        db1.ToDoItems.Add(new ToDoItem { Title = "Call Insurance Company", Completed = false });
        db1.SaveChanges();
    }

    ti = new TenantInfo { Id = "tenant-initech-341ojadsfa", ConnectionString = "mongodb://localhost/isolation-initech", Identifier = "initech" };
    conn = new MongoPerTenantConnection(ti);
    using var db2 = new ToDoDbContext(conn, ti);

    if (!db2.ToDoItems.Any())
    {
        db2.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = false });
        db2.ToDoItems.Add(new ToDoItem { Title = "Pay Salaries", Completed = true });
        db2.ToDoItems.Add(new ToDoItem { Title = "Write Memo", Completed = false });
        db2.SaveChanges();
    }
}

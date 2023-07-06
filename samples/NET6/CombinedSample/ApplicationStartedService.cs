﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataIsolationSample.Data;
using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoFramework;

namespace CombinedSample;

/// <summary>
/// Seed the database the multi-tenant store we'll need.
/// When application has started
/// </summary>
public class ApplicationStartedService : IHostedService
{
    private readonly IMultiTenantStore<MongoTenantInfo> _store;
    private readonly IConfiguration _config;

    public ApplicationStartedService(IMultiTenantStore<MongoTenantInfo> store, IConfiguration config)
    {
        _store = store;
        _config = config;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SetupStore(_store);
        await SetupDb(_store, _config);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Execute code here that you want to run when the application stops
        Console.WriteLine("Application is stopping.");

        return Task.CompletedTask;
    }

    private async Task SetupStore(IMultiTenantStore<MongoTenantInfo> store)
    {
        if (store.GetAllAsync().Result.Any()) return;

        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle" });
        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-tenant-initech" });
        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc" });
    }

    private async Task SetupDb(IMultiTenantStore<MongoTenantInfo> store, IConfiguration config)
    {
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
            await db1.SaveChangesAsync();
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
            await db2.SaveChangesAsync();
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
            await db3.SaveChangesAsync();
        }
    }

}

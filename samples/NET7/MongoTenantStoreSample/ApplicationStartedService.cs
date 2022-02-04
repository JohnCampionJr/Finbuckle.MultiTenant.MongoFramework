// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Hosting;

namespace MongoTenantStoreSample;

/// <summary>
/// Seed the database the multi-tenant store we'll need.
/// When application has started
/// </summary>
public class ApplicationStartedService : IHostedService
{
    private readonly IMultiTenantStore<MongoTenantInfo> _store;

    public ApplicationStartedService(IMultiTenantStore<MongoTenantInfo> store)
    {
        _store = store;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SetupStore(_store);
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
}

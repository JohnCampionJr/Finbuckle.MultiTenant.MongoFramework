// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MongoTenantStoreSample;

/// <summary>
/// Seed the database the multi-tenant store we'll need.
/// When application has started
/// </summary>
public static class SeedService
{
    public static async Task Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var store = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();
        await SetupStore(store);
    }

    private static async Task SetupStore(IMultiTenantStore<MongoTenantInfo> store)
    {
        if (store.GetAllAsync().Result.Any()) return;

        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle" });
        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-tenant-initech" });
        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc" });
    }
}

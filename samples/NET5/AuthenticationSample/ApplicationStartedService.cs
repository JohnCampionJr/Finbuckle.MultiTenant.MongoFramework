// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Hosting;

namespace AuthenticationSample;

public class ApplicationStartedService : IHostedService
{
    private readonly IMultiTenantStore<SampleTenantInfo> _store;

    public ApplicationStartedService(IMultiTenantStore<SampleTenantInfo> store)
    {
        _store = store;
    }
    #region Implementation of IHostedService

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SetupStore(_store);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion

    private async Task SetupStore(IMultiTenantStore<SampleTenantInfo> store)
    {
        if ((await store.GetAllAsync()).Any()) return;

        await store.TryAddAsync(new SampleTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle" });
        await store.TryAddAsync(new SampleTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-auth-initech" });
        await store.TryAddAsync(new SampleTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc" });
    }
}

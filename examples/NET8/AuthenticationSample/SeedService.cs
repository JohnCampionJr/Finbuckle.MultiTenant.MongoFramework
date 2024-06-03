using System.Linq;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationSample;

/// <summary>
/// Seed the database the multi-tenant store we'll need.
/// When application has started
/// </summary>
public static class SeedService
{
    public static async Task Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var store = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<SampleTenantInfo>>();
        await SetupStore(store);
    }

    private static async Task SetupStore(IMultiTenantStore<SampleTenantInfo> store)
    {
        if (store.GetAllAsync().Result.Any()) return;

        await store.TryAddAsync(new SampleTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle" });
        await store.TryAddAsync(new SampleTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-tenant-initech" });
        await store.TryAddAsync(new SampleTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc" });
    }
}

using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using MongoFramework;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides builder methods for Finbuckle.MultiTenant services and configuration.
/// </summary>
public static class FinbuckleMultiTenantBuilderExtensions
{
    /// <summary>
    /// Adds a MongoFramework based multitenant store to the application. 
    /// </summary>
    /// <returns>The same MultiTenantBuilder passed into the method.</returns>
    public static FinbuckleMultiTenantBuilder<TTenantInfo> WithMongoFrameworkStore<TTenantInfo>(this FinbuckleMultiTenantBuilder<TTenantInfo> builder)
        where TTenantInfo : class, ITenantInfo, new()
    {
        return builder.WithStore<MongoTenantStore<TTenantInfo>>(ServiceLifetime.Scoped);
    }

    public static FinbuckleMultiTenantBuilder<TTenantInfo> WithMongoFrameworkStore<TTenantInfo>(this FinbuckleMultiTenantBuilder<TTenantInfo> builder, string connectionString)
        where TTenantInfo : class, ITenantInfo, new()
    {
        return WithMongoFrameworkStore<TTenantInfo, MongoTenantStoreContext>(builder, connectionString);
    }

    public static FinbuckleMultiTenantBuilder<TTenantInfo> WithMongoFrameworkStore<TTenantInfo, TContext>(this FinbuckleMultiTenantBuilder<TTenantInfo> builder, string connectionString)
        where TTenantInfo : class, ITenantInfo, new()
        where TContext : class, IMongoTenantStoreContext
    {
        builder.Services.AddScoped<IMongoTenantStoreConnection>(sp => new MongoTenantStoreConnection(connectionString));
        builder.Services.AddScoped<IMongoTenantStoreContext, TContext>();

        return builder.WithStore<MongoTenantStore<TTenantInfo>>(ServiceLifetime.Scoped);
    }
}

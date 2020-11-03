using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides builder methods for Finbuckle.MultiTenant services and configuration.
    /// </summary>
    public static class FinbuckleMultiTenantBuilderExtensions
    {
        /// <summary>
        /// Adds a MongoFramework based multitenant store to the application. 
        /// </summary>
        /// <returns>The same MultiTenantBuilder passed into the method.</returns>
        public static FinbuckleMultiTenantBuilder<TTenantInfo> WithMongoDbStore<TTenantInfo>(this FinbuckleMultiTenantBuilder<TTenantInfo> builder,
            string defaultConnectionString)
            where TTenantInfo : class, ITenantInfo, new()
        {
            return builder.WithStore<MongoTenantStore<TTenantInfo>>(ServiceLifetime.Scoped, defaultConnectionString);
        }

        public static FinbuckleMultiTenantBuilder<TTenantInfo> WithMongoDbStore<TTenantInfo>(this FinbuckleMultiTenantBuilder<TTenantInfo> builder)
            where TTenantInfo : class, ITenantInfo, new()
        {
            return builder.WithStore<MongoTenantStore<TTenantInfo>>(ServiceLifetime.Scoped);
        }
    }
}

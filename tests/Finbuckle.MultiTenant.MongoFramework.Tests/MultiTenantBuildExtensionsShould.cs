using System;
using System.Linq;
using Finbuckle.MultiTenant.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;
using Shouldly;
using Xunit;

namespace Finbuckle.MultiTenant.Tests
{
    public class MultiTenantBuildExtensionsShould
    {

        [Fact]
        public void RegisterStore()
        {
            var services = new ServiceCollection();
            services.AddTransient<IMongoTenantStoreConnection>(s => new MongoTenantStoreConnection("mongodb://localhost"));
            services.AddTransient<IMongoTenantStoreContext, MongoTenantStoreContext>();

            var builder = new FinbuckleMultiTenantBuilder<MongoTenantInfo>(services);
            builder.WithMongoFrameworkStore();

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();
                db.ShouldBeOfType<MongoTenantStore<MongoTenantInfo>>();
            }
        }

        [Fact]
        public void RegisterStoreWithConnString()
        {
            var services = new ServiceCollection();

            var builder = new FinbuckleMultiTenantBuilder<MongoTenantInfo>(services);
            builder.WithMongoFrameworkStore("mongodb://localhost");

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();
                db.ShouldBeOfType<MongoTenantStore<MongoTenantInfo>>();
                var conn = scoped.ServiceProvider.GetRequiredService<IMongoTenantStoreConnection>();
                conn.ShouldBeOfType<MongoTenantStoreConnection>();
                var context = scoped.ServiceProvider.GetRequiredService<IMongoTenantStoreContext>();
                context.ShouldBeOfType<MongoTenantStoreContext>();
            }
        }

    }
}

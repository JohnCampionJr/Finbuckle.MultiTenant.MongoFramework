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
        public void RegisterStoreWithoutDefaultConnString()
        {
            var services = new ServiceCollection();
            services.AddTransient<IMongoDbConnection>(s =>
            {
                var connection = MongoDbConnection.FromConnectionString("mongodb://localhost");
                return connection;
            });
            services.AddTransient<IMongoDbContext, MongoDbContext>();
            
            var builder = new FinbuckleMultiTenantBuilder<MongoTenantInfo>(services);
            builder.WithMongoDbStore();

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();
                db.ShouldBeOfType<MongoTenantStore<MongoTenantInfo>>();
            }
        }
        
        [Fact]
        public void RegisterStoreWithDefaultConnString()
        {
            var services = new ServiceCollection();
            services.AddTransient<IMongoDbConnection>(s =>
            {
                var connection = MongoDbConnection.FromConnectionString("mongodb://localhost");
                return connection;
            });
            services.AddTransient<IMongoDbContext, MongoDbContext>();
            
            var builder = new FinbuckleMultiTenantBuilder<MongoTenantInfo>(services);
            builder.WithMongoDbStore("mongodb://localhost");

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();
                db.ShouldBeOfType<MongoTenantStore<MongoTenantInfo>>();
            }
        }

    }
}

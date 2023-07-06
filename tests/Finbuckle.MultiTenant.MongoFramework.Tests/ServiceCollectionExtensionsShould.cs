using System;
using System.Linq;
using Finbuckle.MultiTenant.Stores;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;
using Shouldly;
using Xunit;

namespace Finbuckle.MultiTenant.Tests
{
    public class ServiceCollectionExtensionsShould
    {

        [Fact]
        public void RegisterPerTenantConnectionWithoutDefaultConnString()
        {
            var services = new ServiceCollection();
            services.AddMongoPerTenantConnection<MongoPerTenantConnection>();
            services.AddScoped<ITenantInfo, MongoTenantInfo>(s => new MongoTenantInfo { Identifier = "test", ConnectionString = "mongodb://tenant" });

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMongoPerTenantConnection>();
                db.ShouldBeOfType<MongoPerTenantConnection>();
                db.Client.Settings.Server.Host.ShouldBe("tenant");
            }
        }

        [Fact]
        public void RegisterPerTenantConnectionWithOptions()
        {
            var services = new ServiceCollection();
            services.AddMongoPerTenantConnection<MongoPerTenantConnection>(o => { o.DefaultConnectionString = "mongodb://localhost"; });
            services.AddScoped<ITenantInfo, MongoTenantInfo>(s => new MongoTenantInfo { Identifier = "test" });

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMongoPerTenantConnection>();
                db.ShouldBeOfType<MongoPerTenantConnection>();
                db.Client.Settings.Server.Host.ShouldBe("localhost");
            }
        }

        [Fact]
        public void RegisterPerTenantConnectionDefaultWithOptions()
        {
            var services = new ServiceCollection();
            services.AddMongoPerTenantConnection(o => { o.DefaultConnectionString = "mongodb://localhost"; });
            services.AddScoped<ITenantInfo, MongoTenantInfo>(s => new MongoTenantInfo { Identifier = "test" });

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMongoPerTenantConnection>();
                db.ShouldBeOfType<MongoPerTenantConnection>();
                db.Client.Settings.Server.Host.ShouldBe("localhost");
            }
        }

        [Fact]
        public void RegisterPerTenantConnectionDefaultWithDefaultConnString()
        {
            var services = new ServiceCollection();
            services.AddMongoPerTenantConnection("mongodb://localhost");
            services.AddScoped<ITenantInfo, MongoTenantInfo>(s => new MongoTenantInfo { Identifier = "test" });

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMongoPerTenantConnection>();
                db.ShouldBeOfType<MongoPerTenantConnection>();
                db.Client.Settings.Server.Host.ShouldBe("localhost");
            }
        }

        [Fact]
        public void RegisterPerTenantConnectionWithDefaultConnString()
        {
            var services = new ServiceCollection();
            services.AddMongoPerTenantConnection<MongoPerTenantConnection>("mongodb://localhost");
            services.AddScoped<ITenantInfo, MongoTenantInfo>(s => new MongoTenantInfo { Identifier = "test" });

            var provider = services.BuildServiceProvider();
            using (var scoped = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scoped.ServiceProvider.GetRequiredService<IMongoPerTenantConnection>();
                db.ShouldBeOfType<MongoPerTenantConnection>();
                db.Client.Settings.Server.Host.ShouldBe("localhost");
            }
        }

    }
}

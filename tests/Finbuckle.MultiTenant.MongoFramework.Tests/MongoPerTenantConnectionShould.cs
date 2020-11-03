using System;
using Finbuckle.MultiTenant.Stores;
using MongoFramework;
using Shouldly;
using Xunit;

namespace Finbuckle.MultiTenant.Tests
{
    public class MongoPerTenantConnectionShould
    {
        private IMultiTenantStore<MongoTenantInfo> CreateTestStore()
        {
            var conn = new MongoTenantStoreConnection("mongodb://localhost/TenantTests");
            conn.GetDatabase().DropCollection("Tenants");
            var context = new MongoTenantStoreContext(conn);
            var store = new MongoTenantStore<MongoTenantInfo>(context);
            return PopulateTestStore(store);
        }

        private IMultiTenantStore<MongoTenantInfo> PopulateTestStore(IMultiTenantStore<MongoTenantInfo> store)
        {
            _ = store.TryAddAsync(new MongoTenantInfo
            {
                Id = "initech-id", Identifier = "initech", Name = "Initech", ConnectionString = "connstring"
            }).Result;
            _ = store.TryAddAsync(new MongoTenantInfo
            {
                Id = "lol-id", Identifier = "lol", Name = "Lol, Inc.", ConnectionString = "connstring2"
            }).Result;
            _ = store.TryAddAsync(new MongoTenantInfo
            {
                Id = "default-id", Identifier = "default", Name = "Default Conn, Inc."
            }).Result;

            return store;
        }

        [Fact]
        public void SetsUrlWithTenantConnectionString()
        {
            var ti = new MongoTenantInfo
            {
                Id = "initech-id", Identifier = "initech", Name = "Initech", ConnectionString = "mongodb://localhost"
            };

            var conn = new MongoPerTenantConnection(ti);

            conn.Url.Url.ShouldBe("mongodb://localhost");
            conn.TenantInfo.ShouldBeSameAs(ti);
        }

        [Fact]
        public void SetsUrlWithDefaultConnectionString()
        {
            var ti = new MongoTenantInfo
            {
                Id = "initech-id",
                Identifier = "initech",
                Name = "Initech",
            };

            var options = Microsoft.Extensions.Options.Options.Create(new MongoPerTenantConnectionOptions() { DefaultConnectionString = "mongodb://localhost" });

            var conn = new MongoPerTenantConnection(ti, options);

            conn.Url.Url.ShouldBe("mongodb://localhost");
            conn.TenantInfo.ShouldBeSameAs(ti);
        }

        [Fact]
        public void ThrowWithNoTenant()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                _ = new MongoPerTenantConnection(null);
            });
        }

        [Fact]
        public void ThrowWithNoConnectionString()
        {
            var ti = new MongoTenantInfo
            {
                Id = "initech-id", Identifier = "initech", Name = "Initech"
            };

            Should.Throw<ArgumentException>(() =>
            {
                _ = new MongoPerTenantConnection(ti);
            });
        }

        [Fact]
        public void ThrowsExceptionWithInvalidConnectionString()
        {
            var ti = new MongoTenantInfo
            {
                Id = "initech-id", Identifier = "initech", Name = "Initech", ConnectionString = "conn_string"
            };

            Should.Throw<ArgumentException>(() =>
            {
                _ = new MongoPerTenantConnection(ti);
            });
        }

    }
}

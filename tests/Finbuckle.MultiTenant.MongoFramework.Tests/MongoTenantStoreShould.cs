using System;
using System.Linq;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Stores;
using MongoFramework;
using Shouldly;
using Xunit;

namespace Finbuckle.MultiTenant.Tests
{
    public class MongoTenantStoreShould
    {
        private static IMultiTenantStore<MongoTenantInfo> CreateTestStore()
        {
            var conn = new MongoTenantStoreConnection("mongodb://localhost/TenantTests");
            conn.GetDatabase().DropCollection("Tenants");
            var context = new MongoTenantStoreContext(conn);
            var store = new MongoTenantStore<MongoTenantInfo>(context);
            return MongoTenantStoreShould.PopulateTestStore(store);
        }

        private static IMultiTenantStore<MongoTenantInfo> PopulateTestStore(IMultiTenantStore<MongoTenantInfo> store)
        {
            _ = store.AddAsync(new MongoTenantInfo
            {
                Id = "initech-id",
                Identifier = "initech",
                Name = "Initech",
                ConnectionString = "connstring"
            }).Result;
            _ = store.AddAsync(new MongoTenantInfo
            {
                Id = "lol-id",
                Identifier = "lol",
                Name = "Lol, Inc.",
                ConnectionString = "connstring2"
            }).Result;
            _ = store.AddAsync(new MongoTenantInfo
            {
                Id = "default-id",
                Identifier = "default",
                Name = "Default Conn, Inc."
            }).Result;

            return store;
        }

        [Fact]
        public void GetTenantInfoFromStoreById()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            store.GetAsync("initech-id").Result!.Identifier.ShouldBe("initech");
        }

        [Fact]
        public void ReturnNullWhenGettingByIdIfTenantInfoNotFound()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            store.GetAsync("fake123").Result.ShouldBeNull();
        }

        [Fact]
        public void GetTenantInfoFromStoreByIdentifier()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            store.GetByIdentifierAsync("initech").Result!.Identifier.ShouldBe("initech");
        }

        [Fact]
        public void ReturnNullWhenGettingByIdentifierIfTenantInfoNotFound()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            store.GetByIdentifierAsync("fake123").Result.ShouldBeNull();
        }

        [Fact]
        public void AddTenantInfoToStore()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            store.GetByIdentifierAsync("identifier").Result.ShouldBeNull();
            store.AddAsync(new MongoTenantInfo { Id = "id", Identifier = "identifier", Name = "name", ConnectionString = "cs" }).Result.ShouldBeTrue();
            store.GetByIdentifierAsync("identifier").Result.ShouldNotBeNull();
        }

        [Fact]
        public void RemoveTenantInfoFromStore()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            store.GetByIdentifierAsync("initech").Result.ShouldNotBeNull();
            store.RemoveAsync("initech").Result.ShouldBeTrue();
            store.GetByIdentifierAsync("initech").Result.ShouldBeNull();
        }

        [Fact]
        public void ReturnNullWhenRemovingIfTenantInfoNotFound()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            store.RemoveAsync("fake123").Result.ShouldBeFalse();
        }

        [Fact]
        public void UpdateTenantInfoInStore()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            var result = store.UpdateAsync(new MongoTenantInfo { Id = "initech-id", Identifier = "initech2", Name = "Initech2", ConnectionString = "connstring2" }).Result;
            result.ShouldBeTrue();
        }

        [Fact]
        public void ReturnNullWhenUpdatingIfTenantInfoNotFound()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            var fakeTenant = new MongoTenantInfo { Id = "fake-id", Identifier = "fake" };

            store.UpdateAsync(fakeTenant).Result.ShouldBeFalse();
        }

        [Fact]
        public void NotAllowTenantContext()
        {
            var conn = new MongoTenantStoreConnection("mongodb://localhost/TenantTests");
            conn.GetDatabase().DropCollection("Tenants");
            var context = new MongoTestTenantContext(conn, "test");
            Should.Throw<ArgumentException>(() =>
            {
                _ = new MongoTenantStore<MongoTenantInfo>(context);
            });
        }

        [Fact]
        public void GetAllTenants()
        {
            var store = MongoTenantStoreShould.CreateTestStore();
            var list = store.GetAllAsync().Result;
            list.Count().ShouldBe(3);
        }

        private class MongoTestTenantContext : MongoDbTenantContext, IMongoTenantStoreContext
        {
            public MongoTestTenantContext(IMongoTenantStoreConnection conn, string tenantId) : base(conn, tenantId) { }
        }
    }
}

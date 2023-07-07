using System;
using Finbuckle.MultiTenant.Stores;
using MongoFramework;
using Shouldly;
using Xunit;

namespace Finbuckle.MultiTenant.Tests
{
    public class MongoPerTenantConnectionShould
    {

        [Fact]
        public void SetsUrlWithTenantConnectionString()
        {
            var ti = new MongoTenantInfo
            {
                Id = "initech-id",
                Identifier = "initech",
                Name = "Initech",
                ConnectionString = "mongodb://localhost"
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
                Id = "initech-id",
                Identifier = "initech",
                Name = "Initech"
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
                Id = "initech-id",
                Identifier = "initech",
                Name = "Initech",
                ConnectionString = "conn_string"
            };

            Should.Throw<ArgumentException>(() =>
            {
                _ = new MongoPerTenantConnection(ti);
            });
        }

    }
}

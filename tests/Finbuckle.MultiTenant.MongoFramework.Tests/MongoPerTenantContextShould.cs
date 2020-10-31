using MongoFramework;
using Shouldly;
using Xunit;

namespace Finbuckle.MultiTenant.Tests
{
    public class MongoPerTenantContextShould
    {
        [Fact]
        public void SetTenantInfoWithConstructor()
        {
            var ti = new MongoTenantInfo
            {
                Id = "initech-id", Identifier = "initech", Name = "Initech", ConnectionString = "mongodb://localhost/"
            };
            var conn = new MongoPerTenantConnection(ti);
            var context = new MongoPerTenantContext(conn, ti);
            
            context.Connection.Client.Settings.Server.Host.ShouldBe("localhost");
            context.TenantInfo.ShouldBeSameAs(ti);
        }
        
    }
}

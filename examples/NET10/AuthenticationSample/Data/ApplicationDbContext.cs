using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using AuthenticationSample.Data;
using MongoFramework;

namespace AuthenticationSample.Data
{
    public class ApplicationDbContext : MongoPerTenantContext
    {
        public ApplicationDbContext(IMongoPerTenantConnection connection, ITenantInfo ti) : base(connection, ti) { }

        public MongoDbTenantSet<MongoTenantIdentityUser> Users { get; set; }
    }
}

using System;
using MongoFramework;

namespace Finbuckle.MultiTenant.MongoDb
{

    public class MongoTenantStoreContext : MongoDbContext, IMongoTenantStoreContext<MongoTenantInfo>
    {
        public MongoTenantStoreContext(IMongoDbConnection connection) : base(connection)
        {

        }

        public MongoDbSet<MongoTenantInfo> Tenants { get; set; }
    }
}

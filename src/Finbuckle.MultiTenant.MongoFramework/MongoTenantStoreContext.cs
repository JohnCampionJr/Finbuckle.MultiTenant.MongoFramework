// These are essentially empty classes to allow a more narrow dependency injection that just taking any available Context / Connection in the tenant store

namespace MongoFramework;

public interface IMongoTenantStoreContext : IMongoDbContext { }

public class MongoTenantStoreContext : MongoDbContext, IMongoTenantStoreContext
{
    public MongoTenantStoreContext(IMongoTenantStoreConnection connection) : base(connection) { }
}

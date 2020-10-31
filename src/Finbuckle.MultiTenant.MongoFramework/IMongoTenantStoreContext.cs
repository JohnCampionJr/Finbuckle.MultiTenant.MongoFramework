using MongoFramework;

namespace Finbuckle.MultiTenant.MongoDb
{
    public interface IMongoTenantStoreContext<TEntity> : IMongoDbContext where TEntity : class, ITenantInfo
    {
        MongoDbSet<TEntity> Tenants { get; }
    }
}

using Finbuckle.MultiTenant;
using MongoFramework;

// ReSharper disable once CheckNamespace
namespace MongoFramework;

public class MongoPerTenantContext : MongoDbTenantContext
{

    // can't pass null as tenant ID, but the context is still created for the controller prior to the no tenant redirection
    public MongoPerTenantContext(IMongoPerTenantConnection connection, ITenantInfo ti) : base(connection, ti?.Id ?? "")
    {
        TenantInfo = ti;
    }

    public ITenantInfo TenantInfo { get; }
}

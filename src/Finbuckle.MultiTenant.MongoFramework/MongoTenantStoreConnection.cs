// These are essentially empty classes to allow a more narrow dependency injection that just taking any available Context / Connection in the tenant store

// ReSharper disable once CheckNamespace
using MongoDB.Driver;

namespace MongoFramework;

public interface IMongoTenantStoreConnection : IMongoDbConnection { }

public class MongoTenantStoreConnection : MongoDbConnection, IMongoTenantStoreConnection
{
    public MongoTenantStoreConnection(string connectionString)
    {
        this.Url = MongoUrl.Create(connectionString);
    }
}

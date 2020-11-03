using System;
using Finbuckle.MultiTenant;
using MongoDB.Driver;
using MongoFramework.Infrastructure;
using MongoFramework.Infrastructure.Diagnostics;
using MongoFramework.Utilities;

// ReSharper disable once CheckNamespace
namespace MongoFramework
{
    /// <summary>
    /// A MongoDbConnection that accepts a TenantInfo and uses its connection string to create the Data Context
    /// </summary>
    public interface IMongoPerTenantConnection: IMongoDbConnection
    {
        ITenantInfo TenantInfo { get; }
    }
    public class MongoPerTenantConnection : MongoDbConnection, IMongoPerTenantConnection
    {
        public ITenantInfo TenantInfo { get; }

        public MongoPerTenantConnection(ITenantInfo ti)
        {
            Check.NotNull(ti, nameof(ti));
            if (!string.IsNullOrEmpty(ti.ConnectionString) && ti.ConnectionString.ToLower().StartsWith("mongodb://"))
            {
                Url = new MongoUrl(ti.ConnectionString);
                TenantInfo = ti;
            }
            else
            {
                throw new ArgumentException("Connection String required.");
            }
        }
    }
}

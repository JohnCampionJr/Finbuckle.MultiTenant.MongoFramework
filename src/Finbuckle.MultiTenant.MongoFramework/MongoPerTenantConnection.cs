using System;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoFramework.Infrastructure;
using MongoFramework.Infrastructure.Diagnostics;
using MongoFramework.Utilities;

// ReSharper disable once CheckNamespace
namespace MongoFramework;

/// <summary>
/// A MongoDbConnection that accepts a TenantInfo and uses its connection string to create the Data Context
/// </summary>
public interface IMongoPerTenantConnection : IMongoDbConnection
{
    ITenantInfo TenantInfo { get; }
}

public class MongoPerTenantConnection : MongoDbConnection, IMongoPerTenantConnection
{
    public ITenantInfo TenantInfo { get; }

    public MongoPerTenantConnection(ITenantInfo ti, IOptions<MongoPerTenantConnectionOptions> options = null)
    {
        Check.NotNull(ti, nameof(ti));
        TenantInfo = ti;
        if (IsMongoDbConnectionString(ti.ConnectionString))
        {
            Url = new MongoUrl(ti.ConnectionString);
        }
        else if (IsMongoDbConnectionString(options?.Value?.DefaultConnectionString))
        {
            Url = new MongoUrl(options.Value.DefaultConnectionString);
        }
        else
        {
            throw new ArgumentException("Connection String required.");
        }
    }

    private static bool IsMongoDbConnectionString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        return value.ToLowerInvariant().StartsWith("mongodb://") || value.ToLowerInvariant().StartsWith("mongodb+srv://");
    }
}

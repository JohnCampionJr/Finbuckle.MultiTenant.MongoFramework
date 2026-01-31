using System;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoFramework.Utilities;

// ReSharper disable once CheckNamespace
namespace MongoFramework;

/// <summary>
/// Interface for tenant info that includes a connection string for per-tenant database connections.
/// </summary>
public interface IHasConnectionString
{
    string? ConnectionString { get; }
}

/// <summary>
/// A MongoDbConnection that accepts a TenantInfo and uses its connection string to create the Data Context
/// </summary>
public interface IMongoPerTenantConnection : IMongoDbConnection
{
    ITenantInfo? TenantInfo { get; }
}

public class MongoPerTenantConnection : MongoDbConnection, IMongoPerTenantConnection
{
    public ITenantInfo? TenantInfo { get; }

    public MongoPerTenantConnection(ITenantInfo ti, IOptions<MongoPerTenantConnectionOptions>? options = null)
    {
        Check.NotNull(ti, nameof(ti));
        TenantInfo = ti;

        var connectionString = (ti as IHasConnectionString)?.ConnectionString;

        if (IsMongoDbConnectionString(connectionString))
        {
            Url = new MongoUrl(connectionString);
        }
        else if (IsMongoDbConnectionString(options?.Value?.DefaultConnectionString))
        {
            Url = new MongoUrl(options!.Value.DefaultConnectionString!);
        }
        else
        {
            throw new ArgumentException("Connection String required.");
        }
    }

    private static bool IsMongoDbConnectionString(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        return value.StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase) ||
               value.StartsWith("mongodb+srv://", StringComparison.OrdinalIgnoreCase);
    }
}

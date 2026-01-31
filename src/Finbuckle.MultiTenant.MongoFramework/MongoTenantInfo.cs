using System.ComponentModel.DataAnnotations.Schema;
using Finbuckle.MultiTenant.Abstractions;
using MongoFramework;
using MongoFramework.Attributes;

// ReSharper disable once CheckNamespace
namespace Finbuckle.MultiTenant;

/// <summary>
/// This is an optional TenantInfo optimized for MongoDb, adding an index to the Identifier field.
/// </summary>
[Table("Tenants")]
public class MongoTenantInfo : ITenantInfo, IHasConnectionString
{
    public string Id { get; set; } = null!;

    [Index("Identifier", IndexSortOrder.Ascending, IsUnique = true)]
    public string Identifier { get; set; } = null!;
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
}

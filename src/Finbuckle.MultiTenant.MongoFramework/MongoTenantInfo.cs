using System.ComponentModel.DataAnnotations.Schema;
using MongoFramework;
using MongoFramework.Attributes;

// ReSharper disable once CheckNamespace
namespace Finbuckle.MultiTenant;

/// <summary>
/// This is an optional TenantInfo optimized for MongoDb, adding an index to the Identifier field.
/// </summary>
[Table("Tenants")]
public class MongoTenantInfo : ITenantInfo
{
    public string Id { get; set; }

    [Index("Identifier", IndexSortOrder.Ascending, IsUnique = true)]
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}

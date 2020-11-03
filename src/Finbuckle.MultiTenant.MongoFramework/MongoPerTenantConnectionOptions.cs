using MongoFramework.Infrastructure.Diagnostics;

// ReSharper disable once CheckNamespace
namespace MongoFramework
{
    public class MongoPerTenantConnectionOptions
    {
        public string DefaultConnectionString { get; set; }
        public IDiagnosticListener DiagnosticListener { get; set; }
    }
}


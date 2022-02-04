using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using MongoFramework;

namespace DataIsolationSample.Data
{
    public class ToDoDbContext : MongoPerTenantContext
    {
        public ToDoDbContext(IMongoPerTenantConnection connection, ITenantInfo ti) : base(connection, ti) { }
        public MongoDbTenantSet<ToDoItem> ToDoItems { get; set; }
    }
}

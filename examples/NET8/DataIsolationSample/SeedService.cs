using System.Linq;
using System.Threading.Tasks;
using DataIsolationSample.Data;
using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using MongoFramework;

namespace MongoTenantStoreSample;

/// <summary>
/// Seed the database the multi-tenant store we'll need.
/// When application has started
/// </summary>
public static class SeedService
{
    public static async Task Seed()
    {
        await SetupDb();
    }
    
    private static async Task SetupDb()
    {
        var ti = new TenantInfo { Id = "tenant-finbuckle-d043favoiaw", ConnectionString = "mongodb://localhost/isolation-test", Identifier = "finbuckle" };
        var conn = new MongoPerTenantConnection(ti);
        using var db = new ToDoDbContext(conn, ti);
        if (!db.ToDoItems.Any())
        {
            db.ToDoItems.Add(new ToDoItem { Title = "Call Lawyer ", Completed = false });
            db.ToDoItems.Add(new ToDoItem { Title = "File Papers", Completed = false });
            db.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
            await db.SaveChangesAsync();
        }

        ti = new TenantInfo { Id = "tenant-megacorp-g754dafg", ConnectionString = "mongodb://localhost/isolation-test", Identifier = "megacorp" };
        conn = new MongoPerTenantConnection(ti);
        using var db1 = new ToDoDbContext(conn, ti);
        if (!db1.ToDoItems.Any())
        {
            db1.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
            db1.ToDoItems.Add(new ToDoItem { Title = "Construct Additional Pylons", Completed = true });
            db1.ToDoItems.Add(new ToDoItem { Title = "Call Insurance Company", Completed = false });
            await db1.SaveChangesAsync();
        }

        ti = new TenantInfo { Id = "tenant-initech-341ojadsfa", ConnectionString = "mongodb://localhost/isolation-initech", Identifier = "initech" };
        conn = new MongoPerTenantConnection(ti);
        using var db2 = new ToDoDbContext(conn, ti);

        if (!db2.ToDoItems.Any())
        {
            db2.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = false });
            db2.ToDoItems.Add(new ToDoItem { Title = "Pay Salaries", Completed = true });
            db2.ToDoItems.Add(new ToDoItem { Title = "Write Memo", Completed = false });
            await db2.SaveChangesAsync();
        }
    }

}

using System.Linq;
using System.Threading.Tasks;
using DataIsolationSample.Data;
using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

namespace CombinedSample;

/// <summary>
/// Seed the database the multi-tenant store we'll need.
/// When application has started
/// </summary>
public static class SeedService
{
    public static async Task Seed(WebApplication app)
    {
        var config = app.Services.GetRequiredService<IConfiguration>();
        using var scope = app.Services.CreateScope();

        var store = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();
        await SetupStore(store);
        await SetupDb(store, config);
    }

    private static async Task SetupStore(IMultiTenantStore<MongoTenantInfo> store)
    {
        if (store.GetAllAsync().Result.Any()) return;

        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle" });
        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-tenant-initech" });
        await store.TryAddAsync(new MongoTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc" });
    }

    private static async Task SetupDb(IMultiTenantStore<MongoTenantInfo> store, IConfiguration config)
    {
        var ti = store.TryGetByIdentifierAsync("finbuckle").Result;
        if (ti.ConnectionString is null)
            ti.ConnectionString = config.GetConnectionString("DefaultPerTenantConnection");

        var conn = new MongoPerTenantConnection(ti);
        using var db1 = new ToDoDbContext(conn, ti);
        if (!db1.ToDoItems.Any())
        {
            db1.ToDoItems.Add(new ToDoItem { Title = "Call Lawyer ", Completed = false });
            db1.ToDoItems.Add(new ToDoItem { Title = "File Papers", Completed = false });
            db1.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
            await db1.SaveChangesAsync();
        }

        ti = store.TryGetByIdentifierAsync("megacorp").Result;
        if (ti.ConnectionString is null) ti.ConnectionString = config.GetConnectionString("DefaultPerTenantConnection");
        conn = new MongoPerTenantConnection(ti);
        using var db2 = new ToDoDbContext(conn, ti);
        if (!db2.ToDoItems.Any())
        {
            db2.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
            db2.ToDoItems.Add(new ToDoItem { Title = "Construct Additional Pylons", Completed = true });
            db2.ToDoItems.Add(new ToDoItem { Title = "Call Insurance Company", Completed = false });
            await db2.SaveChangesAsync();
        }

        ti = store.TryGetByIdentifierAsync("initech").Result;
        if (ti.ConnectionString is null) ti.ConnectionString = config.GetConnectionString("DefaultPerTenantConnection");
        conn = new MongoPerTenantConnection(ti);
        using var db3 = new ToDoDbContext(conn, ti);
        if (!db3.ToDoItems.Any())
        {
            db3.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = false });
            db3.ToDoItems.Add(new ToDoItem { Title = "Pay Salaries", Completed = true });
            db3.ToDoItems.Add(new ToDoItem { Title = "Write Memo", Completed = false });
            await db3.SaveChangesAsync();
        }
    }

}

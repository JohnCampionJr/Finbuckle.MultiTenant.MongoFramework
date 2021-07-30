using System;
using System.Linq;
using DataIsolationSample.Data;
using DataIsolationSample.Models;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

namespace DataIsolationSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddMultiTenant<MongoTenantInfo>()
                .WithMongoFrameworkStore(Configuration.GetConnectionString("TenantStoreConnection"))
                .WithRouteStrategy()
                .WithRedirectStrategy("/notenant/index");

            services.AddMongoPerTenantConnection(Configuration.GetConnectionString("DefaultPerTenantConnection"));

            // Register the db context, but do not specify a provider/connection string since
            // these vary by tenant.
            services.AddMongoDbContext<ToDoDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseMultiTenant();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{__tenant__}/{controller=Home}/{action=Index}");
                endpoints.MapControllerRoute("notenant", "/{controller=NoTenant}/{action=Index}/{id?}");
            });

            // Seed the database the multitenant store will need.
            SetupStore(app.ApplicationServices);
            SetupDb(app.ApplicationServices);
        }

        private void SetupStore(IServiceProvider sp)
        {
            var scopeServices = sp.CreateScope().ServiceProvider;
            var store = scopeServices.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();

            if (store.GetAllAsync().Result.Any()) return;

            store.TryAddAsync(new MongoTenantInfo{Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle"}).Wait();
            store.TryAddAsync(new MongoTenantInfo{Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-tenant-initech"}).Wait();
            store.TryAddAsync(new MongoTenantInfo{Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc"}).Wait();
        }


        private void SetupDb(IServiceProvider sp)
        {
            var scopeServices = sp.CreateScope().ServiceProvider;
            var store = scopeServices.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();

            var ti = store.TryGetByIdentifierAsync("finbuckle").Result;
            if (ti.ConnectionString is null) ti.ConnectionString = Configuration.GetConnectionString("DefaultPerTenantConnection");

            var conn = new MongoPerTenantConnection(ti);
            using (var db = new ToDoDbContext(conn, ti))
            {
                if (!db.ToDoItems.Any())
                {
                    db.ToDoItems.Add(new ToDoItem {Title = "Call Lawyer ", Completed = false});
                    db.ToDoItems.Add(new ToDoItem {Title = "File Papers", Completed = false});
                    db.ToDoItems.Add(new ToDoItem {Title = "Send Invoices", Completed = true});
                    db.SaveChanges();
                }
            }
            ti = store.TryGetByIdentifierAsync("megacorp").Result;
            if (ti.ConnectionString is null) ti.ConnectionString = Configuration.GetConnectionString("DefaultPerTenantConnection");
            conn = new MongoPerTenantConnection(ti);
            using (var db = new ToDoDbContext(conn, ti))
            {
                if (!db.ToDoItems.Any())
                {
                    db.ToDoItems.Add(new ToDoItem {Title = "Send Invoices", Completed = true});
                    db.ToDoItems.Add(new ToDoItem {Title = "Construct Additional Pylons", Completed = true});
                    db.ToDoItems.Add(new ToDoItem {Title = "Call Insurance Company", Completed = false});
                    db.SaveChanges();
                }
            }
            ti = store.TryGetByIdentifierAsync("initech").Result;
            if (ti.ConnectionString is null) ti.ConnectionString = Configuration.GetConnectionString("DefaultPerTenantConnection");
            conn = new MongoPerTenantConnection(ti);
            using (var db = new ToDoDbContext(conn, ti))
            {
                if (!db.ToDoItems.Any())
                {
                    db.ToDoItems.Add(new ToDoItem {Title = "Send Invoices", Completed = false});
                    db.ToDoItems.Add(new ToDoItem {Title = "Pay Salaries", Completed = true});
                    db.ToDoItems.Add(new ToDoItem {Title = "Write Memo", Completed = false});
                    db.SaveChanges();
                }
            }
        }

    }
}

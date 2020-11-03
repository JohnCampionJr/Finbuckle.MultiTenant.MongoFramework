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

            services.AddMultiTenant<TenantInfo>()
                .WithConfigurationStore()
                .WithRouteStrategy()
                .WithRedirectStrategy("/notenant");

            services.AddScoped<IMongoPerTenantConnection, MongoPerTenantConnection>();

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

            SetupDb();
        }

        private void SetupDb()
        {

            var ti = new TenantInfo { Id = "tenant-finbuckle-d043favoiaw", ConnectionString = "mongodb://localhost/isolation-test", Identifier = "finbuckle" };
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
            ti = new TenantInfo { Id = "tenant-megacorp-g754dafg", ConnectionString = "mongodb://localhost/isolation-test", Identifier = "megacorp" };
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
            ti = new TenantInfo { Id = "tenant-initech-341ojadsfa", ConnectionString = "mongodb://localhost/isolation-initech", Identifier = "initech" };
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

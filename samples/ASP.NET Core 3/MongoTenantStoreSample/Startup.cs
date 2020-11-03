using System;
using System.Linq;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoFramework;

namespace MongoTenantStoreSample
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
                    .WithRouteStrategy();
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
                endpoints.MapControllerRoute("default", "{__tenant__=}/{controller=Home}/{action=Index}");
            });

            // Seed the database the multitenant store will need.
            SetupStore(app.ApplicationServices);
        }

        private void SetupStore(IServiceProvider sp)
        {
            var scopeServices = sp.CreateScope().ServiceProvider;
            var store = scopeServices.GetRequiredService<IMultiTenantStore<MongoTenantInfo>>();

            if (store.GetAllAsync().Result.Any()) return;

            store.TryAddAsync(new MongoTenantInfo{Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle", ConnectionString = "finbuckle_conn_string"}).Wait();
            store.TryAddAsync(new MongoTenantInfo{Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "initech_conn_string"}).Wait();
            store.TryAddAsync(new MongoTenantInfo{Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc", ConnectionString = "megacorp_conn_string"}).Wait();
        }
    }
}

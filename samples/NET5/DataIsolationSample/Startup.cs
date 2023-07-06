using DataIsolationSample.Data;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                .WithRedirectStrategy("/notenant/index");

            services.AddScoped<IMongoPerTenantConnection, MongoPerTenantConnection>();

            // Register the db context, but do not specify a provider/connection string since
            // these vary by tenant.
            services.AddMongoDbContext<ToDoDbContext>();

            //NOTE: this service will be called by the runtime when application is actually started
            services.AddSingleton<IHostedService, ApplicationStartedService>();
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

        }
    }
}

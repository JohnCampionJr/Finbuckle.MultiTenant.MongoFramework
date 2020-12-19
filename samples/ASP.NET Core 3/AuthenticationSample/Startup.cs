using AuthenticationSample.Data;
using Finbuckle.MultiTenant;
using Finbuckle.Utilities.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register per tenant connection to be used in multi tenant context
            services.AddMongoPerTenantConnection(Configuration.GetConnectionString("DefaultPerTenantConnection"));

            // Register the db context, but do not specify a provider/connection
            // string since these vary by tenant.
            services.AddMongoDbContext<ApplicationDbContext>();

            services.AddDefaultIdentity<MongoTenantIdentityUser>()
                    .AddMongoFrameworkStores<ApplicationDbContext>();

            // these wire up Finbuckle to work with pages correctly using the route strategy. If another strategy (host) is used, these would not be necessary
            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.Add(new MultiTenantPageRouteModelConvention());
            });
            services.DecorateService<LinkGenerator, AmbientValueLinkGenerator>(new List<string> { "__tenant__" });

            services.AddMultiTenant<SampleTenantInfo>()
                    .WithRouteStrategy()
                    .WithRedirectStrategy("/notenant/notenant")
                    .WithMongoFrameworkStore(Configuration.GetConnectionString("TenantStoreConnection"))
                    .WithPerTenantAuthentication();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseMultiTenant();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // Seed the database the multitenant store will need.
            SetupStore(app.ApplicationServices);
        }

        private void SetupStore(IServiceProvider sp)
        {
            var scopeServices = sp.CreateScope().ServiceProvider;
            var store = scopeServices.GetRequiredService<IMultiTenantStore<SampleTenantInfo>>();

            if (store.GetAllAsync().Result.Any()) return;

            store.TryAddAsync(new SampleTenantInfo { Id = "tenant-finbuckle-d043favoiaw", Identifier = "finbuckle", Name = "Finbuckle" }).Wait();
            store.TryAddAsync(new SampleTenantInfo { Id = "tenant-initech-341ojadsfa", Identifier = "initech", Name = "Initech LLC", ConnectionString = "mongodb://localhost/samples-auth-initech" }).Wait();
            store.TryAddAsync(new SampleTenantInfo { Id = "tenant-megacorp-g754dafg", Identifier = "megacorp", Name = "MegaCorp Inc" }).Wait();
        }



    }
}

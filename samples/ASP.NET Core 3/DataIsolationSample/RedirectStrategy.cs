using System;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Strategies;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Finbuckle.MultiTenant.Strategies
{
    public class RedirectStrategy : IMultiTenantStrategy
    {
        private readonly string _redirectUrl;

        public int Priority { get => -1000; }

        public RedirectStrategy(string redirectUrl)
        {
            this._redirectUrl = redirectUrl;
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext httpContext))
            {
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));
            }

            if (!httpContext.Request.Path.ToString().ToLower().StartsWith(_redirectUrl.ToLower()))
            {
                httpContext.Response.Redirect(_redirectUrl);
            }

            return await Task.FromResult(default(string));
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides builder methods for Finbuckle.MultiTenant services and configuration.
    /// </summary>
    public static partial class FinbuckleMultiTenantBuilderExtensions
    {
        /// <summary>
        /// Adds and configures a RedirectStrategy to the application.  Should be the last strategy added.
        /// </summary>
        /// <param name="redirectUrl">The path-based Url to redirect users to if no tenants found.</param>
        /// <returns>The same MultiTenantBuilder passed into the method.</returns>
        public static FinbuckleMultiTenantBuilder<TTenantInfo> WithRedirectStrategy<TTenantInfo>(this FinbuckleMultiTenantBuilder<TTenantInfo> builder, string redirectUrl) where TTenantInfo : class, ITenantInfo, new()
        {
            return builder.WithStrategy<RedirectStrategy>(ServiceLifetime.Singleton, redirectUrl);
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Finbuckle.MultiTenant.Strategies;

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

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Strategies;

namespace Microsoft.Extensions.DependencyInjection;

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

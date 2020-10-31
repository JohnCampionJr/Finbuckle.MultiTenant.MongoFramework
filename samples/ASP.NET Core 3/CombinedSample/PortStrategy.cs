//    Copyright 2018-2020 Andrew White
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.Internal;
using Finbuckle.MultiTenant.Strategies;
using Finbuckle.MultiTenant;

namespace Finbuckle.MultiTenant.Strategies
{
    public class PortStrategy : IMultiTenantStrategy
    {
        public PortStrategy() { }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (!(context is HttpContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type HttpContext", nameof(context)));

            var host = (context as HttpContext).Request.Host;

            if (host.HasValue == false || host.Port.HasValue == false)
                return null;

            string identifier = host.Port.ToString();

            return await Task.FromResult(identifier);
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
        /// Adds and configures a PortStrategy to the application.
        /// </summary>
        /// <returns>The same MultiTenantBuilder passed into the method.</returns>
        public static FinbuckleMultiTenantBuilder<TTenantInfo> WithPortStrategy<TTenantInfo>(this FinbuckleMultiTenantBuilder<TTenantInfo> builder) where TTenantInfo : class, ITenantInfo, new()
        {
            return builder.WithStrategy<PortStrategy>(ServiceLifetime.Singleton);
        }

    }
}
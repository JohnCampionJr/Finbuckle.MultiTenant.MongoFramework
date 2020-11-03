using System;
using MongoFramework;
using MongoFramework.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoPerTenantConnection(
                this IServiceCollection serviceCollection,
                Action<MongoPerTenantConnectionOptions> optionsAction = null,
                ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            return AddMongoPerTenantConnection<IMongoPerTenantConnection, MongoPerTenantConnection>(serviceCollection, optionsAction, contextLifetime);
        }

        public static IServiceCollection AddMongoPerTenantConnection<TContext>(
                this IServiceCollection serviceCollection,
                Action<MongoPerTenantConnectionOptions> optionsAction = null,
                ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
                where TContext : IMongoPerTenantConnection
        {
            return AddMongoPerTenantConnection<IMongoPerTenantConnection, TContext>(serviceCollection, optionsAction, contextLifetime);
        }

        public static IServiceCollection AddMongoPerTenantConnection<TContextService, TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<MongoPerTenantConnectionOptions> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : IMongoPerTenantConnection, TContextService
        {

            Check.NotNull(serviceCollection, nameof(serviceCollection));

            if (optionsAction != null)
            {
                var options = new MongoPerTenantConnectionOptions();
                optionsAction?.Invoke(options);

                serviceCollection.Configure<MongoPerTenantConnectionOptions>(o =>
                {
                    o.DefaultConnectionString = options.DefaultConnectionString;
                    o.DiagnosticListener = options.DiagnosticListener;
                });
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(TContextService), typeof(TContextImplementation), contextLifetime));

            return serviceCollection;
        }

        public static IServiceCollection AddMongoPerTenantConnection(
        this IServiceCollection serviceCollection,
        string defaultConnectionString,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            return AddMongoPerTenantConnection<IMongoPerTenantConnection, MongoPerTenantConnection>(serviceCollection, defaultConnectionString, contextLifetime);
        }

        public static IServiceCollection AddMongoPerTenantConnection<TConnection>(
                this IServiceCollection serviceCollection,
                string defaultConnectionString,
                ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
                where TConnection : IMongoPerTenantConnection
        {
            return AddMongoPerTenantConnection<IMongoPerTenantConnection, TConnection>(serviceCollection, defaultConnectionString, contextLifetime);
        }

        public static IServiceCollection AddMongoPerTenantConnection<TConnectionService, TConnectionImplementation>(
            this IServiceCollection serviceCollection,
            string defaultConnectionString,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
            where TConnectionImplementation : IMongoPerTenantConnection, TConnectionService
        {

            Check.NotNull(serviceCollection, nameof(serviceCollection));

            if (!string.IsNullOrEmpty(defaultConnectionString))
            {
                serviceCollection.Configure<MongoPerTenantConnectionOptions>(o =>
                {
                    o.DefaultConnectionString = defaultConnectionString;
                });
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(TConnectionService), typeof(TConnectionImplementation), contextLifetime));

            return serviceCollection;
        }
    }
}


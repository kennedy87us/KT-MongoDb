namespace Kent.MongoDb.Extensions
{
    using Kent.MongoDb.Abstractions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    ///     Specifies the extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds MongoDb context, configuration and index manager.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMongoDb(this IServiceCollection services)
        {
            return AddMongoDb(services, nameof(MongoConfiguration));
        }

        /// <summary>
        ///     Adds MongoDb context, configuration and index manager with given named option.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMongoDb(this IServiceCollection services, string name)
        {
            services.TryAddSingleton<IMongoIndexManager, MongoIndexManager>();
            services.TryAddSingleton<IMongoContextFactory, MongoContextFactory>();
            services.AddSingleton(
                provider =>
                {
                    var factory = provider.GetService<IMongoContextFactory>();
                    return factory.CreateContext(name);
                });

            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            services.Configure<MongoConfiguration>(name, configuration.GetSection(name));

            return services;
        }
    }
}
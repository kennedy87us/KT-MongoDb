namespace Kent.MongoDb.Tests
{
    using Microsoft.Extensions.Configuration;

    internal static class Helpers
    {
        public static void InitConfiguration<T>(T settings)
        {
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build()
                .GetSection(typeof(T).Name).Bind(settings);
        }
    }
}
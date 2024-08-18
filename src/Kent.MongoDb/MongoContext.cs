namespace Kent.MongoDb
{
    using Kent.MongoDb.Abstractions;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using PluralizeService.Core;
    using System;
    using System.Linq;

    /// <summary>
    ///     Represents a type for MongoDb context.
    /// </summary>
    public class MongoContext : IMongoContext
    {
        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="name">The name of context instance.</param>
        /// <param name="configuration">Database configuration.</param>
        public MongoContext(string name, MongoConfiguration configuration)
        {
            SetContext(name, configuration);

            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumStringConvention", pack, t => true);
        }

        /// <summary>
        ///     Gets context name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets client instance.
        /// </summary>
        public IMongoClient Client { get; private set; }

        /// <summary>
        ///     Gets database instance.
        /// </summary>
        public IMongoDatabase Database { get; private set; }

        internal void SetContext(string name, MongoConfiguration configuration)
        {
            this.Name = name;
            this.Client = new MongoClient(configuration.ConnectionString);
            this.Database = this.Client.GetDatabase(configuration.DatabaseName);
        }

        /// <summary>
        ///     Gets collection by document type (collection name will be in plural).
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <returns>An implementation of a collection.</returns>
        public IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            return this.Database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        /// <summary>
        ///     Gets collection by given name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>An implementation of a collection.</returns>
        public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return this.Database.GetCollection<TDocument>(collectionName);
        }

        private static string GetCollectionName(Type type)
        {
            string name = type.Name;

            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                name = genericDef.Name.Replace($"`{genericDef.GetGenericArguments().Length}", string.Empty);
            }

            return PluralizationProvider.Pluralize(name);
        }
    }
}
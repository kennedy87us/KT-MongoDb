namespace Kent.MongoDb
{
    using Kent.MongoDb.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Represents a type that can support to deal with Mongo indexes.
    /// </summary>
    public sealed class MongoIndexManager : IMongoIndexManager
    {
        private readonly IMongoContext _mongoContext;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="mongoContext">The database context.</param>
        public MongoIndexManager(IMongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        /// <summary>
        ///     Creates indexes if not exist.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="dictIndexes">The dictionary of indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The list of index names was created.</returns>
        public IList<string> CreateIndexesIfMissing<TDocument>(Dictionary<string, List<string>> dictIndexes, string collectionName = null)
        {
            var mongoIndexes = dictIndexes.Select(pair => new MongoIndex(pair.Key, pair.Value.Select(fieldName => new MongoIndexField(fieldName)))).ToList();

            return this.CreateIndexesIfMissing<TDocument>(mongoIndexes, collectionName);
        }

        /// <summary>
        ///     Creates indexes if not exist.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="mongoIndexes">The list of mongo indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The list of index names was created.</returns>
        public IList<string> CreateIndexesIfMissing<TDocument>(IEnumerable<IMongoIndex> mongoIndexes, string collectionName = null)
        {
            var indexesNames = new List<string>();
            var collection = collectionName != null ? _mongoContext.GetCollection<TDocument>(collectionName) : _mongoContext.GetCollection<TDocument>();

            foreach (var mongoIndex in mongoIndexes)
            {
                var state = GetIndexExistingState<TDocument>(mongoIndex, collectionName);

                switch (state)
                {
                    case MongoIndexExistingState.AlreadyExists:
                        continue;
                    case MongoIndexExistingState.ExistsWithSameName:
                        collection.Indexes.DropOne(mongoIndex.Name, new DropIndexOptions());
                        break;
                }

                var jsonQuery = "{ " + mongoIndex.Fields.Select(field => field.ToString()).Aggregate((a, b) => $"{a}, {b}") + " }";
                var indexKeyDefinition = new JsonIndexKeysDefinition<TDocument>(jsonQuery);
                var indexModel = new CreateIndexModel<TDocument>(indexKeyDefinition, new CreateIndexOptions { Name = mongoIndex.Name, ExpireAfter = mongoIndex.ExpireAfter });

                var indexName = collection.Indexes.CreateOne(indexModel);
                indexesNames.Add(indexName);
            }

            return indexesNames;
        }

        /// <summary>
        ///     Gets existing state of an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be checked.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The existing state of the index.</returns>
        public MongoIndexExistingState GetIndexExistingState<TDocument>(IMongoIndex index, string collectionName = null)
        {
            if (index == null)
            {
                throw new ArgumentNullException(nameof(index));
            }

            MongoIndexExistingState result = default;
            var collection = collectionName != null ? _mongoContext.GetCollection<TDocument>(collectionName) : _mongoContext.GetCollection<TDocument>();
            var jsonQuery = "{ " + index.Fields.Select(field => field.ToString()).Aggregate((a, b) => $"{a}, {b}") + " }";
            var existIndexes = collection.Indexes.List().ToList();
            var foundIndex = existIndexes.SingleOrDefault(document => document.Elements.Any(element => element.Name == "name" && element.Value == index.Name));

            if (foundIndex != null)
            {
                var indexKeysAsJson = foundIndex.Elements.Single(element => element.Name == "key").Value.ToJson();
                var expectedIndexKeysAsJson = new JsonIndexKeysDefinition<TDocument>(jsonQuery).Json;

                if (indexKeysAsJson == expectedIndexKeysAsJson)
                {
                    result = MongoIndexExistingState.AlreadyExists;
                }
                else
                {
                    result = MongoIndexExistingState.ExistsWithSameName;
                }
            }
            else
            {
                var listIndexKeysAsJson = existIndexes.Select(document => document.Elements.Single(element => element.Name == "key").Value.ToJson()).ToList();
                var expectedIndexKeysAsJson = new JsonIndexKeysDefinition<TDocument>(jsonQuery).Json;
                if (listIndexKeysAsJson.Contains(expectedIndexKeysAsJson))
                {
                    result = MongoIndexExistingState.ExistsWithDifferentName;
                }
            }

            return result;
        }

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void DropAllIndexes<TDocument>(string collectionName = null, CancellationToken cancellationToken = default)
        {
            DropAllIndexes<TDocument>(default, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="options">The options for dropping indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void DropAllIndexes<TDocument>(DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default)
        {
            var collection = collectionName != null ? _mongoContext.GetCollection<TDocument>(collectionName) : _mongoContext.GetCollection<TDocument>();
            collection.Indexes.DropAll(options, cancellationToken);
        }

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        public Task DropAllIndexesAsync<TDocument>(string collectionName = null, CancellationToken cancellationToken = default)
        {
            return DropAllIndexesAsync<TDocument>(default, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="options">The options for dropping indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        public Task DropAllIndexesAsync<TDocument>(DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default)
        {
            var collection = collectionName != null ? _mongoContext.GetCollection<TDocument>(collectionName) : _mongoContext.GetCollection<TDocument>();
            return collection.Indexes.DropAllAsync(options, cancellationToken);
        }

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void DropIndex<TDocument>(IMongoIndex index, string collectionName = null, CancellationToken cancellationToken = default)
        {
            DropIndex<TDocument>(index.Name, default, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void DropIndex<TDocument>(IMongoIndex index, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default)
        {
            DropIndex<TDocument>(index.Name, options, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void DropIndex<TDocument>(string name, string collectionName = null, CancellationToken cancellationToken = default)
        {
            DropIndex<TDocument>(name, default, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void DropIndex<TDocument>(string name, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default)
        {
            var collection = collectionName != null ? _mongoContext.GetCollection<TDocument>(collectionName) : _mongoContext.GetCollection<TDocument>();
            collection.Indexes.DropOne(name, options, cancellationToken);
        }

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        public Task DropIndexAsync<TDocument>(IMongoIndex index, string collectionName = null, CancellationToken cancellationToken = default)
        {
            return DropIndexAsync<TDocument>(index.Name, default, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        public Task DropIndexAsync<TDocument>(IMongoIndex index, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default)
        {
            return DropIndexAsync<TDocument>(index.Name, options, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        public Task DropIndexAsync<TDocument>(string name, string collectionName = null, CancellationToken cancellationToken = default)
        {
            return DropIndexAsync<TDocument>(name, default, collectionName, cancellationToken);
        }

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        public Task DropIndexAsync<TDocument>(string name, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default)
        {
            var collection = collectionName != null ? _mongoContext.GetCollection<TDocument>(collectionName) : _mongoContext.GetCollection<TDocument>();
            return collection.Indexes.DropOneAsync(name, options, cancellationToken);
        }
    }
}
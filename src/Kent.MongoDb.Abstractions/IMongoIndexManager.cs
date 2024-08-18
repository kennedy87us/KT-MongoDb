namespace Kent.MongoDb.Abstractions
{
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Represents a type that can support to deal with Mongo indexes.
    /// </summary>
    public interface IMongoIndexManager
    {
        /// <summary>
        ///     Creates indexes if not exist.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="dictIndexes">The dictionary of indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The list of index names was created.</returns>
        IList<string> CreateIndexesIfMissing<TDocument>(Dictionary<string, List<string>> dictIndexes, string collectionName = null);

        /// <summary>
        ///     Creates indexes if not exist.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="mongoIndexes">The list of mongo indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The list of index names was created.</returns>
        IList<string> CreateIndexesIfMissing<TDocument>(IEnumerable<IMongoIndex> mongoIndexes, string collectionName = null);

        /// <summary>
        ///     Gets existing state of an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be checked.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The existing state of the index.</returns>
        MongoIndexExistingState GetIndexExistingState<TDocument>(IMongoIndex index, string collectionName = null);

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        void DropAllIndexes<TDocument>(string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="options">The options for dropping indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        void DropAllIndexes<TDocument>(DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropAllIndexesAsync<TDocument>(string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="options">The options for dropping indexes.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropAllIndexesAsync<TDocument>(DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        void DropIndex<TDocument>(IMongoIndex index, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        void DropIndex<TDocument>(IMongoIndex index, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        void DropIndex<TDocument>(string name, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        void DropIndex<TDocument>(string name, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(IMongoIndex index, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(IMongoIndex index, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(string name, string collectionName = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(string name, DropIndexOptions options, string collectionName = null, CancellationToken cancellationToken = default);
    }
}
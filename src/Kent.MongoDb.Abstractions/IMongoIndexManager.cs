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
        ///     Sets database context to use with.
        /// </summary>
        /// <param name="context">The instance of database context</param>
        void SetContext(IMongoContext context);

        /// <summary>
        ///     Sets default database context to use with.
        /// </summary>
        void SetContext();

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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        void DropAllIndexes<TDocument>(CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="options">The options for dropping indexes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        void DropAllIndexes<TDocument>(DropIndexOptions options, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropAllIndexesAsync<TDocument>(CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops all the indexes.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="options">The options for dropping indexes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropAllIndexesAsync<TDocument>(DropIndexOptions options, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        void DropIndex<TDocument>(IMongoIndex index, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        void DropIndex<TDocument>(IMongoIndex index, DropIndexOptions options, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        void DropIndex<TDocument>(string name, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        void DropIndex<TDocument>(string name, DropIndexOptions options, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(IMongoIndex index, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="index">The index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(IMongoIndex index, DropIndexOptions options, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(string name, CancellationToken cancellationToken = default, string collectionName = null);

        /// <summary>
        ///     Drops an index by its name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="name">The name of the index need to be dropped.</param>
        /// <param name="options">The options for dropping index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A task that performs the operation.</returns>
        Task DropIndexAsync<TDocument>(string name, DropIndexOptions options, CancellationToken cancellationToken = default, string collectionName = null);
    }
}
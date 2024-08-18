namespace Kent.MongoDb.Abstractions
{
    using MongoDB.Driver;

    /// <summary>
    ///     Represents a type for MongoDb context.
    /// </summary>
    public interface IMongoContext
    {
        /// <summary>
        ///     Gets context name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets client instance.
        /// </summary>
        IMongoClient Client { get; }

        /// <summary>
        ///     Gets database instance.
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        ///     Gets collection by document type (collection name will be in plural).
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <returns>An implementation of a collection.</returns>
        IMongoCollection<TDocument> GetCollection<TDocument>();

        /// <summary>
        ///     Gets collection by given name.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>An implementation of a collection.</returns>
        IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName);
    }
}
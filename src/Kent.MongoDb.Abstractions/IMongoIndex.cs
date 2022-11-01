namespace Kent.MongoDb.Abstractions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Represents a type for Mongo index.
    /// </summary>
    public interface IMongoIndex
    {
        /// <summary>
        ///     Gets the index name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the list index fields.
        /// </summary>
        IEnumerable<IMongoIndexField> Fields { get; }

        /// <summary>
        ///     Gets when documents expire (used with TTL indexes).
        /// </summary>
        TimeSpan? ExpireAfter { get; }
    }
}
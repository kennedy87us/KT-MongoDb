namespace Kent.MongoDb
{
    using Kent.MongoDb.Abstractions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Represents a type for Mongo index.
    /// </summary>
    public class MongoIndex : IMongoIndex
    {
        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="name">The index name.</param>
        /// <param name="fields">The list index fields.</param>
        /// <param name="expireAfter">The expiry time (used for TTL index).</param>
        public MongoIndex(string name, IEnumerable<IMongoIndexField> fields, TimeSpan? expireAfter = null)
        {
            this.Name = name;
            this.Fields = fields;
            this.ExpireAfter = expireAfter;
        }

        /// <summary>
        ///     Gets the index name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the list index fields.
        /// </summary>
        public IEnumerable<IMongoIndexField> Fields { get; }

        /// <summary>
        ///     Gets when documents expire (used with TTL indexes).
        /// </summary>
        public TimeSpan? ExpireAfter { get; }
    }
}
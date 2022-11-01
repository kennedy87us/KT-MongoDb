namespace Kent.MongoDb.Abstractions
{
    /// <summary>
    ///     Enumerated type for MongoDb index type.
    /// </summary>
    public enum MongoIndexType
    {
        /// <summary>
        ///     Ascending sort direction.
        /// </summary>
        Ascending,

        /// <summary>
        ///     Descending sort direction.
        /// </summary>
        Descending,

        /// <summary>
        ///     Text index which supports text search queries on string content.
        /// </summary>
        Text
    }
}
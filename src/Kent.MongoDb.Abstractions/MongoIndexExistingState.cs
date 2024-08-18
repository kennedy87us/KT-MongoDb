namespace Kent.MongoDb.Abstractions
{
    /// <summary>
    ///     Enumerated type for MongoDb index existing state.
    /// </summary>
    public enum MongoIndexExistingState
    {
        /// <summary>
        ///     Index does not exist.
        /// </summary>
        NotExists,

        /// <summary>
        ///     Index already exists by name and definition.
        /// </summary>
        AlreadyExists,

        /// <summary>
        ///     Index definition already exists with a different name.
        /// </summary>
        ExistsWithDifferentName,

        /// <summary>
        ///     An existing index has the same name as the requested index but they are different from definition.
        /// </summary>
        ExistsWithSameName
    }
}
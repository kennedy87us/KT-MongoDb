namespace Kent.MongoDb.Abstractions
{
    /// <summary>
    ///     Represents a type for Mongo index field.
    /// </summary>
    public interface IMongoIndexField
    {
        /// <summary>
        ///     Gets or sets the field name.
        /// </summary>
        string Name { get; set; }
    }
}
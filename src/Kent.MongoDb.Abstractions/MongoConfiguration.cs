namespace Kent.MongoDb.Abstractions
{
    /// <summary>
    ///     Configuration to connect MongoDb instance.
    /// </summary>
    public class MongoConfiguration
    {
        /// <summary>
        ///     Gets or sets database connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets database name.
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
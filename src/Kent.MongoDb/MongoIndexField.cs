namespace Kent.MongoDb
{
    using Kent.MongoDb.Abstractions;
    using System;

    /// <summary>
    ///     Represents a type for Mongo index field.
    /// </summary>
    public class MongoIndexField : IMongoIndexField
    {
        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="indexType">The index type.</param>
        public MongoIndexField(string name, MongoIndexType indexType = MongoIndexType.Ascending)
        {
            this.Name = name;
            this.IndexType = indexType;
        }

        /// <summary>
        ///     Gets or sets the field name.
        /// </summary>
        public string Name { get; set; }

        private MongoIndexType IndexType { get; }

        private string IndexTypeToString
        {
            get
            {
                return this.IndexType switch
                {
                    MongoIndexType.Ascending => "1",
                    MongoIndexType.Descending => "-1",
                    MongoIndexType.Text => "\"text\"",
                    _ => throw new NotSupportedException($"Index type {this.IndexType} is not supported"),
                };
            }
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"\"{this.Name}\" : {this.IndexTypeToString}";
        }
    }
}
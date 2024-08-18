namespace Kent.MongoDb.Abstractions
{
    using System;

    /// <summary>
    ///     Represents a type that can create instances of <see cref="IMongoContext"/>.
    /// </summary>
    public interface IMongoContextFactory : IDisposable
    {
        /// <summary>
        ///     Creates the instance of <see cref="IMongoContext"/>.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <returns>An implementation of a context.</returns>
        IMongoContext CreateContext(string name);
    }
}
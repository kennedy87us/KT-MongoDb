namespace Kent.MongoDb
{
    using Kent.MongoDb.Abstractions;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Represents a type that can create instances of <see cref="IMongoContext"/>.
    /// </summary>
    public sealed class MongoContextFactory : IMongoContextFactory
    {
        private readonly IOptionsMonitor<MongoConfiguration> _options;
        private readonly Dictionary<string, MongoContext> _contexts;
        private readonly IDisposable _onChangeToken;
        private bool _disposed = false;

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="options">The options to create <see cref="MongoContext"/> instances with.</param>
        public MongoContextFactory(IOptionsMonitor<MongoConfiguration> options)
        {
            _options = options;
            _contexts = new Dictionary<string, MongoContext>();
            _onChangeToken = _options.OnChange(OptionsChanged);
        }

        /// <summary>
        ///     Creates the instance of <see cref="IMongoContext"/>.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <returns>An implementation of a context.</returns>
        public IMongoContext CreateContext(string name)
        {
            if (!_contexts.ContainsKey(name))
            {
                _contexts.Add(name, new MongoContext(name, _options.Get(name)));
            }
            return GetContext(name);
        }

        /// <summary>
        ///     Releases the allocated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases the allocated resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _onChangeToken.Dispose();
                    _contexts.Clear();
                }
            }
            _disposed = true;
        }

        private MongoContext GetContext(string name) => _contexts[name];

        private void OptionsChanged(MongoConfiguration config, string name)
        {
            if (!_contexts.ContainsKey(name))
            {
                _contexts.Add(name, new MongoContext(name, config));
            }
            else
            {
                var context = GetContext(name);
                context.SetContext(name, config);
            }
        }
    }
}
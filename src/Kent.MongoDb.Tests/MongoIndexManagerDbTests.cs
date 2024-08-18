namespace Kent.MongoDb.Tests
{
    using Kent.MongoDb.Abstractions;
    using Kent.MongoDb.Tests.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    [Collection(nameof(NonParallelizationCollectionDefinition))]
    public class MongoIndexManagerDbTests : IDisposable
    {
        private readonly MongoConfiguration _configuration;
        private readonly IMongoContext _mongoContext;
        private readonly IMongoIndexManager _mongoIndexManager;
        private bool _disposed = false;

        public MongoIndexManagerDbTests()
        {
            _configuration = new MongoConfiguration();
            Helpers.InitConfiguration(_configuration);
            _mongoContext = new MongoContext(null, _configuration);
            _mongoIndexManager = new MongoIndexManager(_mongoContext);

            CleanData();
        }

        public static IEnumerable<object[]> MongoIndexTypesValues()
        {
            foreach (var indexType in Enum.GetValues(typeof(MongoIndexType)))
            {
                yield return new[] { indexType };
            }
        }

#if DEBUG
        [Fact]
#else
        [Fact(Skip = "db test only")]
#endif
        public void CreateIndexesIfMissing_NotExists_Then_CreateNewIndex_DefaultAscending()
        {
            // Arrange
            var collection = _mongoContext.GetCollection<Test>();

            var indexName = "TestIndexDefaultAscending";
            var dictIndexes = new Dictionary<string, List<string>>
            {
                { indexName, new List<string> { nameof(Test.Value), nameof(Test.RefId) } }
            };

            // Act
            _mongoIndexManager.CreateIndexesIfMissing<Test>(dictIndexes);
            var actualIndex = GetIndex(collection, indexName);

            // Assert
            // index created
            Assert.NotNull(actualIndex);

            var keys = actualIndex.Elements.SingleOrDefault(e => e.Name == "key").Value;
            Assert.Equal(1, keys.AsBsonDocument.SingleOrDefault(e => e.Name.Equals(nameof(Test.Value))).Value.AsInt32);
            Assert.Equal(1, keys.AsBsonDocument.SingleOrDefault(e => e.Name.Equals(nameof(Test.RefId))).Value.AsInt32);
        }

#if DEBUG
        [Fact]
#else
        [Fact(Skip = "db test only")]
#endif
        public void CreateIndexesIfMissing_NotExists_Then_CreateNewIndex_MixedSort()
        {
            // Arrange
            var collection = _mongoContext.GetCollection<Test>();

            var indexName = "TestIndexMixedSort";
            var mongoIndexes = new List<IMongoIndex>()
            {
                new MongoIndex(
                    indexName,
                    new List<IMongoIndexField> {
                        new MongoIndexField(nameof(Test.Value), MongoIndexType.Ascending),
                        new MongoIndexField(nameof(Test.RefId), MongoIndexType.Descending)
                    })
            };

            // Act
            _mongoIndexManager.CreateIndexesIfMissing<Test>(mongoIndexes);
            var actualIndex = GetIndex(collection, indexName);

            // Assert
            // index created
            Assert.NotNull(actualIndex);

            var keys = actualIndex.Elements.SingleOrDefault(e => e.Name == "key").Value;
            Assert.Equal(1, keys.AsBsonDocument.SingleOrDefault(e => e.Name.Equals(nameof(Test.Value))).Value.AsInt32);
            Assert.Equal(-1, keys.AsBsonDocument.SingleOrDefault(e => e.Name.Equals(nameof(Test.RefId))).Value.AsInt32);
        }

#if DEBUG
        [Fact]
#else
        [Fact(Skip = "db test only")]
#endif
        public void CreateIndexesIfMissing_NotExists_Then_CreateNewIndex_Text()
        {
            // Arrange
            var collection = _mongoContext.GetCollection<Test>();

            var indexName = "TestIndexText";
            var mongoIndexes = new List<IMongoIndex>()
            {
                new MongoIndex(
                    indexName,
                    new List<IMongoIndexField> {
                        new MongoIndexField(nameof(Test.RefId), MongoIndexType.Text)
                    })
            };

            // Act
            _mongoIndexManager.CreateIndexesIfMissing<Test>(mongoIndexes);
            var actualIndex = GetIndex(collection, indexName);

            // Assert
            // index created
            Assert.NotNull(actualIndex);

            var keys = actualIndex.Elements.SingleOrDefault(e => e.Name == "key").Value;
            Assert.Equal(nameof(MongoIndexType.Text).ToLower(), keys.AsBsonDocument.SingleOrDefault(e => e.Name.Equals("_fts")).Value.AsString);

            var weights = actualIndex.Elements.SingleOrDefault(e => e.Name == "weights").Value;
            Assert.Equal(1, weights.AsBsonDocument.SingleOrDefault(e => e.Name.Equals(nameof(Test.RefId))).Value.AsInt32);
        }

#if DEBUG
        [Fact]
#else
        [Fact(Skip = "db test only")]
#endif
        public void CreateIndexesIfMissing_NotExists_Then_CreateNewIndex_TTL()
        {
            // Arrange
            var collection = _mongoContext.GetCollection<Test>();

            var indexName = "TestIndexTTL";
            var mongoIndexes = new List<IMongoIndex>()
            {
                new MongoIndex(
                    indexName,
                    new List<IMongoIndexField> { new MongoIndexField(nameof(Test.RefId)) },
                    TimeSpan.FromSeconds(86400))
            };

            // Act
            _mongoIndexManager.CreateIndexesIfMissing<Test>(mongoIndexes);
            var actualIndex = GetIndex(collection, indexName);

            // Assert
            // index created
            Assert.NotNull(actualIndex);

            var keys = actualIndex.Elements.SingleOrDefault(e => e.Name == "key").Value;
            Assert.Equal(1, keys.AsBsonDocument.SingleOrDefault(e => e.Name.Equals(nameof(Test.RefId))).Value.AsInt32);

            var ttlIndex = actualIndex.Elements.FirstOrDefault(e => e.Name == "expireAfterSeconds").Value;
            Assert.NotNull(ttlIndex);
        }

#if DEBUG
        [Fact]
#else
        [Fact(Skip = "db test only")]
#endif
        public void CreateIndexesIfMissing_SameName_DifferentFields_Then_ReplaceNewIndex()
        {
            // Arrange
            var collection = _mongoContext.GetCollection<Test>();

            var indexName = "TestIndexReplace";
            var existDictIndexes = new Dictionary<string, List<string>>
            {
                { indexName, new List<string> { nameof(Test.RefId) } }
            };

            var newDictIndexes = new Dictionary<string, List<string>>
            {
                { indexName, new List<string> { nameof(Test.Value) } }
            };

            // Act
            _mongoIndexManager.CreateIndexesIfMissing<Test>(existDictIndexes);
            var existIndex = GetIndex(collection, indexName);

            _mongoIndexManager.CreateIndexesIfMissing<Test>(newDictIndexes);
            var newlIndex = GetIndex(collection, indexName);

            // Assert
            // index replaced
            Assert.NotNull(newlIndex);
            Assert.NotEqual(existIndex, newlIndex);

            var keys = newlIndex.Elements.SingleOrDefault(e => e.Name == "key").Value;
            Assert.Equal(1, keys.AsBsonDocument.SingleOrDefault(e => e.Name.Equals(nameof(Test.Value))).Value.AsInt32);
        }

#if DEBUG
        [Fact]
#else
        [Fact(Skip = "db test only")]
#endif
        public void CreateIndexesIfMissing_DifferentName_SameFields_ThrowException()
        {
            // Arrange
            var collection = _mongoContext.GetCollection<Test>();

            var existIndexName = "TestIndexExist";
            var existDictIndexes = new Dictionary<string, List<string>>
            {
                { existIndexName, new List<string> { nameof(Test.RefId) } }
            };

            var newIndexName = "TestIndexNew";
            var newDictIndexes = new Dictionary<string, List<string>>
            {
                { newIndexName, new List<string> { nameof(Test.RefId) } }
            };

            // Act
            _mongoIndexManager.CreateIndexesIfMissing<Test>(existDictIndexes);

            // Assert
            // index not created
            Assert.Throws<MongoCommandException>(() => _mongoIndexManager.CreateIndexesIfMissing<Test>(newDictIndexes));
        }

#if DEBUG
        [Theory]
#else
        [Theory(Skip = "db test only")]
#endif
        [MemberData(nameof(MongoIndexTypesValues))]
        public void CreateIndexesIfMissing_CreateEachIndexType_CreateIndex(MongoIndexType mongoIndexType)
        {
            // Arrange
            var collection = _mongoContext.GetCollection<TestType<EntityType>>();

            var indexName = $"Test_{mongoIndexType}";
            var mongoIndexes = new List<MongoIndex>()
            {
                new (indexName, new List<MongoIndexField> {
                    new MongoIndexField<TestType<EntityType>>(d => d.Entity.Type, mongoIndexType)
                })
            };

            // Act
            _mongoIndexManager.CreateIndexesIfMissing<TestType<EntityType>>(mongoIndexes);
            var actualIndex = GetIndex(collection, indexName);

            // Assert
            // index created
            Assert.NotNull(actualIndex);
        }

        private static BsonDocument GetIndex<T>(IMongoCollection<T> collection, string indexName)
        {
            var indexes = collection.Indexes.List().ToList();
            var index = indexes.SingleOrDefault(document => document.Elements.Any(element => element.Name == "name" && element.Value == indexName));
            return index;
        }

        private void CleanData()
        {
            var database = _mongoContext.Database;
            database.DropCollection(_mongoContext.GetCollection<Test>().CollectionNamespace.CollectionName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CleanData();
                }
            }
            _disposed = true;
        }
    }
}
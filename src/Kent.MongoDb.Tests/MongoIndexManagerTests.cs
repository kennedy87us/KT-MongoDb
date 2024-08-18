namespace Kent.MongoDb.Tests
{
    using Kent.MongoDb.Abstractions;
    using Kent.MongoDb.Tests.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class MongoIndexManagerTests
    {
        private readonly Mock<IAsyncCursor<BsonDocument>> _mockAsyncCursor;
        private readonly Mock<IMongoIndexManager<Test>> _mockIndexManager;
        private readonly Mock<IMongoCollection<Test>> _mockCollection;
        private readonly Mock<IMongoContext> _mockContext;

        public MongoIndexManagerTests()
        {
            _mockAsyncCursor = new Mock<IAsyncCursor<BsonDocument>>();
            _mockAsyncCursor.Setup(m => m.Current).Returns(Enumerable.Empty<BsonDocument>());
            _mockAsyncCursor.SetupSequence(m => m.MoveNext(It.IsAny<CancellationToken>()))
                            .Returns(true).Returns(false);
            _mockAsyncCursor.SetupSequence(m => m.MoveNextAsync(It.IsAny<CancellationToken>()))
                            .Returns(Task.FromResult(true)).Returns(Task.FromResult(false));

            _mockIndexManager = new Mock<IMongoIndexManager<Test>>();
            _mockIndexManager.Setup(m => m.List(It.IsAny<CancellationToken>())).Returns(_mockAsyncCursor.Object);
            _mockIndexManager.Setup(m => m.CreateOne(It.IsAny<CreateIndexModel<Test>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()));

            _mockCollection = new Mock<IMongoCollection<Test>>();
            _mockCollection.SetupGet(m => m.Indexes).Returns(_mockIndexManager.Object);

            _mockContext = new Mock<IMongoContext>();
            _mockContext.Setup(m => m.GetCollection<Test>()).Returns(_mockCollection.Object);
        }

        [Fact]
        public void CreateIndexesIfMissing_NotExists_Then_CreateNewIndex()
        {
            // Prepare
            var indexName = "TestIndex";
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            var dictIndexes = new Dictionary<string, List<string>>
            {
                { indexName, new List<string> { nameof(Test.Value), nameof(Test.RefId) } }
            };
            indexManager.CreateIndexesIfMissing<Test>(dictIndexes);

            // Assert
            _mockIndexManager.Verify(m => m.DropOne(It.IsAny<string>(), It.IsAny<DropIndexOptions>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockIndexManager.Verify(m => m.CreateOne(It.IsAny<CreateIndexModel<Test>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public void CreateIndexesIfMissing_SameName_DifferentFields_Then_ReplaceIndex()
        {
            // Prepare
            var indexName = "TestIndex";
            _mockAsyncCursor.Setup(m => m.Current).Returns(new[]
            {
                new BsonDocument(
                    new List<BsonElement> {
                        new ("v", 2),
                        new ("key", new BsonDocument(
                            new List<BsonElement> { new (nameof(Test.Id), 1), new (nameof(Test.Value), 1) })
                        ),
                        new ("name", indexName)
                    }
                )
            });
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            var dictIndexes = new Dictionary<string, List<string>>
            {
                { indexName, new List<string> { nameof(Test.Id), nameof(Test.RefId) } }
            };
            indexManager.CreateIndexesIfMissing<Test>(dictIndexes);

            // Assert
            _mockIndexManager.Verify(m => m.DropOne(It.Is<string>(x => x == indexName), It.IsAny<DropIndexOptions>(), It.IsAny<CancellationToken>()), Times.Once());
            _mockIndexManager.Verify(m => m.CreateOne(It.IsAny<CreateIndexModel<Test>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public void CreateIndexesIfMissing_DifferentName_SameFields_Then_ThrowException()
        {
            // Prepare
            var indexName = "TestIndex";
            _mockAsyncCursor.Setup(m => m.Current).Returns(new[]
            {
                new BsonDocument(
                    new List<BsonElement> {
                        new ("v", 2),
                        new ("key", new BsonDocument(
                            new List<BsonElement> { new (nameof(Test.Id), 1), new (nameof(Test.Value), 1) })
                        ),
                        new ("name", indexName)
                    }
                )
            });
            _mockIndexManager.Setup(m => m.CreateOne(It.IsAny<CreateIndexModel<Test>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()))
                             .Throws<Exception>();
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            var dictIndexes = new Dictionary<string, List<string>>
            {
                { $"{indexName}_new", new List<string> { nameof(Test.Id), nameof(Test.Value) } }
            };
            Assert.Throws<Exception>(() => indexManager.CreateIndexesIfMissing<Test>(dictIndexes));

            // Assert
            _mockIndexManager.Verify(m => m.DropOne(It.IsAny<string>(), It.IsAny<DropIndexOptions>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockIndexManager.Verify(m => m.CreateOne(It.IsAny<CreateIndexModel<Test>>(), It.IsAny<CreateOneIndexOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public void GetIndexExistingState_Throw_Exception()
        {
            // Prepare
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => indexManager.GetIndexExistingState<Test>(null));
        }

        [Fact]
        public void GetIndexExistingState_Returns_NotExists()
        {
            // Prepare
            var indexName = "TestIndex";
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            var index = new MongoIndex(indexName, new List<MongoIndexField>
            {
                new (nameof(Test.Value)),
                new (nameof(Test.RefId))
            });
            var actual = indexManager.GetIndexExistingState<Test>(index);

            // Assert
            Assert.Equal(MongoIndexExistingState.NotExists, actual);
        }

        [Fact]
        public void GetIndexExistingState_Returns_AlreadyExists()
        {
            // Prepare
            var indexName = "TestIndex";
            _mockAsyncCursor.Setup(m => m.Current).Returns(new[]
            {
                new BsonDocument(
                    new List<BsonElement> {
                        new ("v", 2),
                        new ("key", new BsonDocument(
                            new List<BsonElement> { new (nameof(Test.Id), 1), new (nameof(Test.RefId), 1) })
                        ),
                        new ("name", indexName)
                    }
                )
            });
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            var index = new MongoIndex(indexName, new List<MongoIndexField>
            {
                new (nameof(Test.Id)),
                new (nameof(Test.RefId))
            });
            var actual = indexManager.GetIndexExistingState<Test>(index);

            // Assert
            Assert.Equal(MongoIndexExistingState.AlreadyExists, actual);
        }

        [Fact]
        public void GetIndexExistingState_Returns_ExistsWithSameName()
        {
            // Prepare
            var indexName = "TestIndex";
            _mockAsyncCursor.Setup(m => m.Current).Returns(new[]
            {
                new BsonDocument(
                    new List<BsonElement> {
                        new ("v", 2),
                        new ("key", new BsonDocument(
                            new List<BsonElement> { new (nameof(Test.Id), 1), new (nameof(Test.Value), 1) })
                        ),
                        new ("name", indexName)
                    }
                )
            });
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            var index = new MongoIndex(indexName, new List<MongoIndexField>
            {
                new (nameof(Test.Id)),
                new (nameof(Test.RefId))
            });
            var actual = indexManager.GetIndexExistingState<Test>(index);

            // Assert
            Assert.Equal(MongoIndexExistingState.ExistsWithSameName, actual);
        }

        [Fact]
        public void GetIndexExistingState_Returns_ExistsWithDifferentName()
        {
            // Prepare
            var indexName = "TestIndex";
            _mockAsyncCursor.Setup(m => m.Current).Returns(new[]
            {
                new BsonDocument(
                    new List<BsonElement> {
                        new ("v", 2),
                        new ("key", new BsonDocument(
                            new List<BsonElement> { new (nameof(Test.Id), 1), new (nameof(Test.Value), 1) })
                        ),
                        new ("name", indexName)
                    }
                )
            });
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            var index = new MongoIndex($"{indexName}_new", new List<MongoIndexField>
            {
                new (nameof(Test.Id)),
                new (nameof(Test.Value))
            });
            var actual = indexManager.GetIndexExistingState<Test>(index);

            // Assert
            Assert.Equal(MongoIndexExistingState.ExistsWithDifferentName, actual);
        }

        [Fact]
        public void DropAllIndexes_NoException()
        {
            // Prepare
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            indexManager.DropAllIndexes<Test>();
            indexManager.DropAllIndexes<Test>(new DropIndexOptions());

            // Assert
            _mockIndexManager.Verify(m => m.DropAll(It.IsAny<DropIndexOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async void DropAllIndexesAsync_NoException()
        {
            // Prepare
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            await indexManager.DropAllIndexesAsync<Test>();
            await indexManager.DropAllIndexesAsync<Test>(new DropIndexOptions());

            // Assert
            _mockIndexManager.Verify(m => m.DropAllAsync(It.IsAny<DropIndexOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public void DropIndex_NoException()
        {
            // Prepare
            var indexName = "TestIndex";
            var mongoIndex = new MongoIndex(indexName, Enumerable.Empty<IMongoIndexField>());
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            indexManager.DropIndex<Test>(mongoIndex);
            indexManager.DropIndex<Test>(mongoIndex, new DropIndexOptions());
            indexManager.DropIndex<Test>(indexName);
            indexManager.DropIndex<Test>(indexName, new DropIndexOptions());

            // Assert
            _mockIndexManager.Verify(m => m.DropOne(indexName, It.IsAny<DropIndexOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
        }

        [Fact]
        public void DropIndexAsync_NoException()
        {
            // Prepare
            var indexName = "TestIndex";
            var mongoIndex = new MongoIndex(indexName, Enumerable.Empty<IMongoIndexField>());
            var indexManager = new MongoIndexManager(_mockContext.Object);

            // Act
            indexManager.DropIndexAsync<Test>(mongoIndex);
            indexManager.DropIndexAsync<Test>(mongoIndex, new DropIndexOptions());
            indexManager.DropIndexAsync<Test>(indexName);
            indexManager.DropIndexAsync<Test>(indexName, new DropIndexOptions());

            // Assert
            _mockIndexManager.Verify(m => m.DropOneAsync(indexName, It.IsAny<DropIndexOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
        }
    }
}
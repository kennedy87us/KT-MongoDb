namespace Kent.MongoDb.Tests
{
    using Kent.MongoDb.Abstractions;
    using Kent.MongoDb.Tests.Constants;
    using Kent.MongoDb.Tests.Models;
    using Moq;
    using PluralizeService.Core;
    using System.Linq;
    using Xunit;

    [Collection(nameof(NonParallelizationCollectionDefinition))]
    public class MongoContextTests
    {
        private readonly MongoConfiguration _configuration;
        private readonly Mock<IMongoContextFactory> _mockContextFactory;

        public MongoContextTests()
        {
            _configuration = new MongoConfiguration();
            Helpers.InitConfiguration(_configuration);
            _mockContextFactory = new Mock<IMongoContextFactory>();
            _mockContextFactory.Setup(m => m.CreateContext(It.IsAny<string>())).Returns(new MongoContext(string.Empty, _configuration));
        }

        [Fact]
        public void GetCollection_ByType_Then_Return_PluralName()
        {
            // Prepare
            var context = _mockContextFactory.Object.CreateContext(null);

            // Act
            var collection = context.GetCollection<TestType<EntityType>>();

            // Assert
            Assert.Contains(PluralizationProvider.Pluralize(nameof(TestType<EntityType>)), collection.CollectionNamespace.CollectionName);
        }

        [Fact]
        public void GetCollection_ByName_Then_Return_ExactName()
        {
            // Prepare
            var context = _mockContextFactory.Object.CreateContext(null);

            // Act
            var collection = context.GetCollection<Test>(TestConstants.COLLECTION_NAME);

            // Assert
            Assert.Contains(TestConstants.COLLECTION_NAME, collection.CollectionNamespace.CollectionName);
        }
    }
}
namespace Kent.MongoDb.Tests
{
    using Kent.MongoDb.Abstractions;
    using Microsoft.Extensions.Options;
    using Moq;
    using System;
    using Xunit;

    public class MongoContextFactoryTests : IDisposable
    {
        private readonly MongoConfiguration _configuration;
        private readonly Mock<IOptionsMonitor<MongoConfiguration>> _mockOptionsMonitor;
        private readonly IMongoContextFactory _contextFactory;

        public MongoContextFactoryTests()
        {
            _configuration = new MongoConfiguration();
            Helpers.InitConfiguration(_configuration);
            _mockOptionsMonitor = new Mock<IOptionsMonitor<MongoConfiguration>>();
            _contextFactory = new MongoContextFactory(_mockOptionsMonitor.Object);
        }

        [Fact]
        public void CreateContext_Then_Return_Context()
        {
            // Arrange
            _mockOptionsMonitor.Setup(m => m.Get(It.IsAny<string>())).Returns(_configuration);

            // Act
            var actualResult = this._contextFactory.CreateContext("");

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<MongoContext>(actualResult);
        }

        public void Dispose()
        {
            _contextFactory.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
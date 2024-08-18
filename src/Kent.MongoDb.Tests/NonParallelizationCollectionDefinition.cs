namespace Kent.MongoDb.Tests
{
    using Xunit;

    [CollectionDefinition(nameof(NonParallelizationCollectionDefinition), DisableParallelization = true)]
    public class NonParallelizationCollectionDefinition { }
}
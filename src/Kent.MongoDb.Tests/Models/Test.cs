namespace Kent.MongoDb.Tests.Models
{
    public class Test
    {
        public string Id { get; set; }

        public string Value { get; set; }

        public string RefId { get; set; }
    }

    public class TestType<T> : Test where T : EntityType
    {
        public T Entity { get; set; }
    }
}
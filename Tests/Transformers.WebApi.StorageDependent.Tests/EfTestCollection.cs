using Xunit;

namespace Transformers.WebApi.StorageDependent.Tests
{
    [CollectionDefinition(nameof(EfTestCollection))]
    public class EfTestCollection : ICollectionFixture<EfTestFixture>
    {
    }
}

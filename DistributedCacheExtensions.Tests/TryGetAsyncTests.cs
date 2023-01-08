
namespace DistributedCacheExtensions.Tests
{
    public class TryGetAsyncTests
    {
        [Fact]
        public async Task TryGetAsync_WithExistingKey_ReturnsTrueAndExpectedValue()
        {
            var cache = new Mock<IDistributedCache>();
            var key = "key";
            var expectedValue = 123;
            var rs = JsonSerializer.SerializeToUtf8Bytes(expectedValue);

            cache.Setup(c => c.GetAsync(
                It.Is<string>(k => k == key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(rs);

            var (found, value) = await cache.Object.TryGetAsync<int>(key);
            found.ShouldBeTrue();
            value.ShouldBe(expectedValue);
        }

        [Fact]
        public async Task TryGetAsync_WithNonExistingKey_ReturnsFalseAndDefaultValue()
        {
            // Arrange
            var cache = new Mock<IDistributedCache>();
            cache.Setup(c => c.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(byte[]));

            // Act
            var (found, value) = await cache.Object.TryGetAsync<int>("key");

            // Assert
            found.ShouldBeFalse();
            value.ShouldBe(default);
        }
    }
}
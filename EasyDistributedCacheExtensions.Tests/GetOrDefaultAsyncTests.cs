
namespace DistributedCacheExtensions.Tests
{


    public class GetOrDefaultAsyncTests
    {
        [Fact]
        public async Task GetOrDefaultAsync_WithExistingKey_ReturnsExpectedValue()
        {
            var cache = new Mock<IDistributedCache>();
            var key = "key";
            var expectedValue = 123;
            var rs = JsonSerializer.SerializeToUtf8Bytes(expectedValue);

            cache.Setup(c => c.GetAsync(
                It.Is<string>(k => k == key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(rs);

            var value = await cache.Object.GetOrDefaultAsync<int>(key);
            value.ShouldBe(expectedValue);
        }

        [Fact]
        public async Task GetOrDefaultAsync_WithNonExistingKey_ReturnsDefaultValue()
        {
            var cache = new Mock<IDistributedCache>();
            cache.Setup(c => c.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(byte[]));

            var value = await cache.Object.GetOrDefaultAsync<int>("k");
            value.ShouldBe(default);
        }

        [Fact]
        public async Task GetOrDefaultAsync_WithExistingKey_ReturnsSpecifiedDefaultValue()
        {
            // Arrange
            var cache = new Mock<IDistributedCache>();
            var key = "key";
            var defaultValue = 456;

            cache.Setup(c => c.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(byte[]));

            // Act
            var value = await cache.Object.GetOrDefaultAsync<int>(key, defaultValue);

            // Assert
            value.ShouldBe(defaultValue);
        }
    }
}
namespace DistributedCacheExtensions.Tests
{
    public class GetAsyncTests
    {
        public class IDistributedCacheGetAsyncTests
        {
            [Fact]
            public async Task GetAsync_WithExistingKey_ReturnsExpectedValue()
            {
                var cache = new Mock<IDistributedCache>();
                var key = "key";
                var expectedValue = 123;
                var rs = JsonSerializer.SerializeToUtf8Bytes(expectedValue);

                cache.Setup(c => c.GetAsync(
                    It.Is<string>(k => k == key),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(rs);

                var value = await cache.Object.GetAsync<int>(key);
                value.ShouldBe(expectedValue);
            }

            [Fact]
            public async Task GetAsync_WithNonExistingKey_ReturnsDefaultValue()
            {
                // Arrange
                var cache = new Mock<IDistributedCache>();
                cache.Setup(c => c.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync(default(byte[]));

                // Act
                var value = await cache.Object.GetAsync<int>("key");

                // Assert
                value.ShouldBe(default);
            }
        }
    }
}
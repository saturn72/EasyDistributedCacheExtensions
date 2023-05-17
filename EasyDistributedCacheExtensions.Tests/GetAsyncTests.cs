namespace EasyDistributedCacheExtensions.Tests
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

            [Fact]
            public async Task GetAsync_Acquire_Should_Return_Value_From_Cache_If_Present()
            {
                var cache = new Mock<IDistributedCache>();
                var key = "testKey";
                var value = JsonSerializer.SerializeToUtf8Bytes("testValue");
                cache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(value);

                var result = await cache.Object.GetAsync<string>(key, () => throw new Exception("Should not be called"));

                result.ShouldBe("testValue");
            }
        }

        [Fact]
        public async Task GetAsyncAcquire_Should_Call_Acquire_Function_If_Value_Not_Present_In_Cache()
        {
            var cache = new Mock<IDistributedCache>();
            var key = "testKey";
            cache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            cache.Setup(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var exp = "testValue";
            var res = await cache.Object.GetAsync<string>(key, () => Task.FromResult(exp));
            res.ShouldBe(exp);

            var bytes = JsonSerializer.SerializeToUtf8Bytes(exp);
            cache.Verify(c => c.SetAsync(It.Is<string>(s => s == key), It.Is<byte[]>(b => b.SequenceEqual(bytes)), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
            Times.Once);

        }

        [Fact]
        public async Task GetAsyncAcquire_Should_Return_Default_Value_If_Acquire_Function_Returns_Null()
        {
            var cache = new Mock<IDistributedCache>();
            var key = "testKey";
            var value = JsonSerializer.SerializeToUtf8Bytes(default(string));
            cache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(value);

            var result = await cache.Object.GetAsync<string>(key, () => Task.FromResult<string?>(null));

            result.ShouldBeNull();
            cache.Verify(c => c.SetAsync(It.Is<string>(s => s == key), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),
            Times.Never);
        }
        [Fact]
        public async Task GetAsyncAcquire_Should_Store_Value_In_Cache_After_Acquiring_It()
        {
            var cache = new Mock<IDistributedCache>();
            var key = "testKey";
            var value = JsonSerializer.SerializeToUtf8Bytes("testValue");

            cache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);
            cache.Setup(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var exp = "testValue";
            var res = await cache.Object.GetAsync<string>(key, () => Task.FromResult(exp));
            res.ShouldBe(exp);
            cache.Verify(c => c.SetAsync(key, value, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
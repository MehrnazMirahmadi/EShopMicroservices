namespace Basket.Tests;

public class GetBasketEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetBasketEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetBasket_ReturnsCart_WhenBasketExists()
    {
        // Arrange
        var userName = "testuser";
        var cart = new ShoppingCart(userName)
        {
            Items = new List<ShoppingCartItem>
            {
                new() { Quantity = 2, Price = 100 }
            }
        };

        var mockRepo = new Mock<IBasketRepository>();
        mockRepo.Setup(r => r.GetBasket(userName
            , It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IBasketRepository)); 
                services.AddSingleton(mockRepo.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync($"/basket/{userName}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GetBasketResponse>();
        result.Should().NotBeNull();
        result!.Cart.UserName.Should().Be(userName);
        result.Cart.Items.Should().HaveCount(1);
        result.Cart.Items[0].Quantity.Should().Be(2);
        result.Cart.Items[0].Price.Should().Be(100);
    }
}

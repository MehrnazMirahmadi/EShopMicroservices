namespace Basket.Tests;

public class GetBasketQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsBasket_WhenUserHasBasket()
    {
        // Arrange
        var userName = "testuser";
        var fakeBasket = new ShoppingCart(userName)
        {
            Items = new List<ShoppingCartItem>
            {
                new() { Quantity = 2, Price = 100 }
            }
        };

        var mockRepo = new Mock<IBasketRepository>();
        mockRepo.Setup(r => r.GetBasket(userName, CancellationToken.None))
                .ReturnsAsync(fakeBasket);

        var handler = new GetBasketQueryHandler(mockRepo.Object);
        var query = new GetBasketQuery(userName);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Cart.Should().NotBeNull();
        result.Cart.UserName.Should().Be(userName);
        result.Cart.Items.Should().HaveCount(1);
        result.Cart.Items[0].Quantity.Should().Be(2);
        result.Cart.Items[0].Price.Should().Be(100);
    }
}

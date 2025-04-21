namespace Basket.Tests;

public class DeleteBasketEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeleteBasketEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeleteBasket_ReturnsOk_WhenBasketDeletedSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userName = "testuser";

        // Act
        var response = await client.DeleteAsync($"/basket/{userName}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("isSuccess");
        responseContent.Should().Contain("true");
    }

}
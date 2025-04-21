public class GetProductByIdEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetProductByIdEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Should_Return_200_And_Product_When_Found()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var expectedProduct = new Product
        {
            Id = productId,
            Name = "Test Product"
        };

        var senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.Is<GetProductByIdQuery>(q => q.Id == productId), default))
                  .ReturnsAsync(new GetProductByIdResult(expectedProduct));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(senderMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync($"/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<GetProductByIdResponse>();
        responseData.Should().NotBeNull();
        responseData!.Product.Id.Should().Be(expectedProduct.Id);
        responseData.Product.Name.Should().Be(expectedProduct.Name);
    }

    [Fact]
    public async Task Should_Return_404_When_Id_Is_Not_Guid()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/products/not-a-valid-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


}

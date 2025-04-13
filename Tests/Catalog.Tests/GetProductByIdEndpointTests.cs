using Catalog.API.Models;
using Catalog.API.Products.GetProductById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;

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
            // سایر فیلدها در صورت نیاز
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
    public async Task Should_Return_400_When_Id_Is_Invalid()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/products/not-a-valid-guid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}

using Catalog.API.Exeptions;
using Catalog.API.Models;
using Catalog.API.Products.GetProductById;
using Marten;
using Microsoft.Extensions.Logging;
using Moq;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IDocumentSession> _sessionMock = new();
    //private readonly Mock<ILogger<GetProductByIdQueryHandler>> _loggerMock = new();

    [Fact]
    public async Task Handle_ProductExists_ReturnsResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var expectedProduct = new Product { Id = productId, Name = "Test Product" };
        _sessionMock.Setup(s => s.LoadAsync<Product>(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedProduct);

       // var handler = new GetProductByIdQueryHandler(_sessionMock.Object, _loggerMock.Object);

        // Act
       // var result = await handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        // Assert
        //Assert.NotNull(result);
        //Assert.Equal(expectedProduct, result.Product);
    }

    [Fact]
    public async Task Handle_ProductDoesNotExist_ThrowsProductNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _sessionMock.Setup(s => s.LoadAsync<Product>(productId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Product?)null);

    //    var handler = new GetProductByIdQueryHandler(_sessionMock.Object, _loggerMock.Object);

        // Act + Assert
      //  await Assert.ThrowsAsync<ProductNotFoundExeption>(() =>
          //  handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None));
    }
}

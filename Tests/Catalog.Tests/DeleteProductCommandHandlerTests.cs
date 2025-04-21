namespace Catalog.Tests;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IDocumentSession> _sessionMock = new();
    [Fact]
    public async Task Handle_Should_ThrowException_When_ProductNotFound()
    {
        // Arrange
        var sessionMock = new Mock<IDocumentSession>();
        var productId = Guid.NewGuid();

        sessionMock.Setup(s => s.LoadAsync<Product>(productId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Product)null); // محصول وجود ندارد

        var handler = new DeleteProductCommandHandler(sessionMock.Object);
        var command = new DeleteProductCommand(productId);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundExeption>(() =>
            handler.Handle(command, CancellationToken.None));
    }
    [Fact]
    public async Task Handle_Should_DeleteProduct_And_ReturnSuccess()
    {
        // Arrange
        var sessionMock = new Mock<IDocumentSession>();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Sample" };

        sessionMock.Setup(s => s.LoadAsync<Product>(productId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(product);

        var handler = new DeleteProductCommandHandler(sessionMock.Object);
        var command = new DeleteProductCommand(productId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        sessionMock.Verify(s => s.Delete(product), Times.Once);
        sessionMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result.IsSuccess);
    }

}

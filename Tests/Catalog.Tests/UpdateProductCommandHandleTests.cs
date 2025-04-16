using Catalog.API.Exeptions;
using Catalog.API.Models;
using Catalog.API.Products.UpdateProduct;
using Marten;
using Moq;

namespace Catalog.Tests;

public class UpdateProductCommandHandleTests
{
    [Fact]
    public async Task Handle_Should_ThrowException_When_ProductNotFound()
    {
        // Arrange
        var sessionMock = new Mock<IDocumentSession>();
        var productId = Guid.NewGuid();

        sessionMock.Setup(s => s.LoadAsync<Product>(productId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Product)null);

        var handler = new UpdateProductCommandHandler(sessionMock.Object);
        var command = new UpdateProductCommand(
            productId, "Updated Product", new List<string> { "Category1" }, "Description", "image.jpg", 99.99m
        );

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundExeption>(() =>
            handler.Handle(command, CancellationToken.None));
    }

}

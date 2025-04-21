namespace Catalog.Tests;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Product_And_Return_Id()
    {
        // Arrange
        var fakeSession = new Mock<IDocumentSession>();
        Product? storedProduct = null;

        // Mock Store method to capture stored product
        fakeSession
            .Setup(s => s.Store(It.IsAny<Product[]>()))
            .Callback<Product[]>((products) =>
            {
                storedProduct = products.FirstOrDefault(); // دریافت اولین محصول از آرایه
                if (storedProduct != null)
                {
                    storedProduct.Id = Guid.NewGuid(); // مقداردهی به Id برای محصول
                }
            });

        // Mock SaveChangesAsync (do nothing)
        fakeSession
            .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Create handler
        var handler = new CreateProductCommandHandler(fakeSession.Object);

        var command = new CreateProductCommand(
            Name: "Test Product",
            Category: new List<string> { "Category1", "Category2" },
            Description: "Test Description",
            ImageFile: "test.jpg",
            Price: 99.99m
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        storedProduct.ShouldNotBeNull();
        storedProduct!.Name.ShouldBe("Test Product");
        storedProduct.Price.ShouldBe(99.99m);
        storedProduct.Category.ShouldContain("Category1");
        storedProduct.Description.ShouldBe("Test Description");

        result.Id.ShouldNotBe(Guid.Empty);

        // Ensure that session methods were called
        fakeSession.Verify(s => s.Store(It.IsAny<Product[]>()), Times.Once);
        fakeSession.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

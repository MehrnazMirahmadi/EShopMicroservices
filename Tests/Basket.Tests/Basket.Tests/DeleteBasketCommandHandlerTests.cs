namespace Basket.Tests;
public class DeleteBasketCommandHandlerTests
{
    [Fact]
    public async Task Handler_Should_ReturnSuccess_WhenBasketDeleted()
    {
        var userName = "testuser";
        var mockRepo = new Mock<IBasketRepository>();
        mockRepo.Setup(r => r.DeleteBasket(userName, It.IsAny<CancellationToken>()))
         .ReturnsAsync(true)
         .Verifiable();

        var handler = new DeleteBasketCommandHandler(mockRepo.Object);
        var command = new DeleteBasketCommand(userName);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        mockRepo.Verify(r => r.DeleteBasket(userName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Validator_Should_HaveError_WhenUserNameIsEmpty()
    {
        // Arrange
        var validator = new DeleteBasketCommandValidator();
        var command = new DeleteBasketCommand("");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Validator_Should_NotHaveError_WhenUserNameIsProvided()
    {
        // Arrange
        var validator = new DeleteBasketCommandValidator();
        var command = new DeleteBasketCommand("validuser");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.UserName);
    }
    [Fact]
    public async Task DeleteBasket_Should_RemoveBasket_WhenExists()
    {
        // Arrange
        var store = new Dictionary<string, ShoppingCart>
    {
        { "user1", new ShoppingCart("user1") },
        { "user2", new ShoppingCart("user2") }
    };

        var mockRepo = new Mock<IBasketRepository>();

        // Setup DeleteBasket to remove from dictionary
        mockRepo.Setup(r => r.DeleteBasket(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string userName, CancellationToken _) =>
            {
                var removed = store.Remove(userName);
                return removed;
            });

        var handler = new DeleteBasketCommandHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle(new DeleteBasketCommand("user1"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        store.Should().ContainKey("user2");
        store.Should().NotContainKey("user1");
        store.Count.Should().Be(1);
    }


}


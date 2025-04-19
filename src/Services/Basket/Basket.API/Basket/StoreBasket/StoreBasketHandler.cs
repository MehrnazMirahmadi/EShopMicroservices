namespace Basket.API.Basket.StoreBasket;
public record StoreBasketCommand(ShoppingCart Cart): ICommand<StoreBasketResult>;
public record StoreBasketResult(string UserName);
public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart)
            .NotNull()
            .WithMessage("Cart can note be null");
        RuleFor(x => x.Cart.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
    }
}
public class StoreBasketCommandHandler
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        ShoppingCart cart = command.Cart;
        // Todo : store basket in database (use Marten upsert - if exist = update , if not exist insert)
        // Todo : udpate cache
        return new StoreBasketResult("swn");
    }
}

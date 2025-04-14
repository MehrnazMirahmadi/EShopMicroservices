using BuildingBlocks.Exceptions;

namespace Catalog.API.Exeptions;

public class ProductNotFoundExeption :NotFoundException
{
    public ProductNotFoundExeption(Guid Id) : base("Product",Id)
    {
            
    }
}

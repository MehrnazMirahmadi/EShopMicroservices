namespace Catalog.API.Exeptions;

public class ProductNotFoundExeption :Exception
{
    public ProductNotFoundExeption() : base("Product not found!")
    {
            
    }
}

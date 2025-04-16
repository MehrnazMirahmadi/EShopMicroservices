using Xunit;
using Catalog.API.Models;
using Catalog.API.Products.GetProducts;
using Marten;
using Shouldly;

namespace Catalog.Tests;

public class GetProductsQueryHandlerTests
{
    private IDocumentStore CreateStore(string schema)
    {
        return DocumentStore.For(options =>
        {
            options.Connection("Host=localhost;Port=5432;Database=CatalogDb;Username=postgres;Password=postgres");
            options.DatabaseSchemaName = schema;
            options.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All;
        });
    }
    [Fact]
    public async Task Handle_Should_Return_Products()
    {
        // Arrange: ساخت یک store Marten برای PostgreSQL
        var store = CreateStore("products_test");

        // باز کردن سشن و اضافه کردن دیتا
        using (var session = store.LightweightSession())
        {
            session.Store<Product>(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                ImageFile = "test.jpg",
                Price = 100,
                Category = new List<string> { "Books" }
            });

            await session.SaveChangesAsync();
        }

        // Act
        using (var session = store.LightweightSession())
        {
            var handler = new GetProductsQueryHandler(session);
            var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

            // Assert
            result.Products.ShouldNotBeNull();
            result.Products.Count().ShouldBeGreaterThan(0);
        }
    }
    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Products_Exist()
    {
        // Arrange
        var store = CreateStore("products_empty_test");
        using var session = store.LightweightSession();

        // Act
        var handler = new GetProductsQueryHandler(session);
        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        result.Products.ShouldNotBeNull();
        result.Products.ShouldBeEmpty();
    }
    [Fact]
    public async Task Handle_Should_Return_Paginated_Products()
    {
        var store = CreateStore("products_pagination_test");
        // Arrange: 15 محصول ذخیره می‌کنیم
        using (var session = store.LightweightSession())
        {
            for (int i = 1; i <= 15; i++)
            {
                session.Store<Product>(new Product
                {
                    Name = $"Product {i}",
                    Description = $"Description {i}",
                    ImageFile = $"image{i}.jpg",
                    Price = 10 * i,
                    Category = new List<string> { "Books" }
                });
            }

            await session.SaveChangesAsync();
        }

        // Act: درخواست صفحه دوم با اندازه صفحه 10
        using (var session = store.LightweightSession())
        {
            var handler = new GetProductsQueryHandler(session);
            var query = new GetProductsQuery(PageNumber: 2, PageSize: 10);
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Count().ShouldBe(10);
            result.Products.First().Name.ShouldBe("Product 11");
        }
    }
    [Fact]
    public async Task Handle_Should_Use_Default_Paging_When_Query_Parameters_Are_Null()
    {
        // Arrange
        var store = CreateStore("products_default_paging_test");
        using (var session = store.LightweightSession())
        {
            for (int i = 1; i <= 12; i++)
            {
                session.Store(new Product
                {
                    Name = $"Product {i}",
                    Description = $"Description {i}",
                    ImageFile = $"image{i}.jpg",
                    Price = 10 * i,
                    Category = new List<string> { "Default" }
                });
            }

            await session.SaveChangesAsync();
        }

        // Act
        using (var session = store.LightweightSession())
        {
            var handler = new GetProductsQueryHandler(session);
            var query = new GetProductsQuery(); // هیچ پارامتری پاس نمی‌دیم
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Count().ShouldBe(10); // چون PageSize پیش‌فرض ۱۰ هست
            result.Products.First().Name.ShouldBe("Product 1");
        }
    }


}

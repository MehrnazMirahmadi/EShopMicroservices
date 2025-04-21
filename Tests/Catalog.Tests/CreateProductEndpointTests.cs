namespace Catalog.Tests
{
    public class CreateProductEndpointTests
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public CreateProductEndpointTests()
        {

            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();

        }
        //[Fact]
        //public void AlwaysTrue_ShouldPass()
        //{
        //    
        //    Assert.True(true);
        //}
        [Fact]
        public async Task CreateProduct_ValidRequest_ReturnsCreatedResponse()
        {
            var request = new CreateProductRequest(
                Name: "Product1",
                Category: new List<string> { "Category1" },
                Description: "This is a product",
                ImageFile: "image.jpg",
                Price: 99.99m
            );
            var mockSession = new Mock<IDocumentSession>();
            mockSession.Setup(session => session.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var mockProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Category = new List<string> { "Category1" },
                Description = "This is a product",
                ImageFile = "image.jpg",
                Price = 99.99m
            };

            var handler = new CreateProductCommandHandler(mockSession.Object);

            var response = await _client.PostAsJsonAsync("/products", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreateProductResponse>();

            Assert.NotEqual(Guid.Empty, result.Id);
        }


    }
}

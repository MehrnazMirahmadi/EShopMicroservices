using Catalog.API.Products.GetProducts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace Catalog.Tests;

public class GetProductsEndPointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public GetProductsEndPointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task GetProducts_Should_Return_OK_And_Products()
    {
        var response = await _client.GetAsync("/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);


        var result = await response.Content.ReadFromJsonAsync<GetProductsResult>();
        result.ShouldNotBeNull();
        result.Products.ShouldNotBeEmpty();
    }

}

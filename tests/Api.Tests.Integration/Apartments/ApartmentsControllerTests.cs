using System.Net.Http.Json;
using Api.Dtos;
using Domain.Apartments;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.Apartments;

public class ApartmentsControllerTests : BaseIntegrationTest
{
    public ApartmentsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ShouldCreateApartment()
    {
        // Arrange
        var request = new CreateApartmentDto(
            Address: "Test apartment",
            Area: 55.5m,
            RoomsCount: 2,
            Price: 100_000m);

        // Act
        var response = await Client.PostAsJsonAsync("apartments", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseDto = await response.Content.ReadFromJsonAsync<ApartmentDto>();
        responseDto.Should().NotBeNull();

        var dbApartment = await Context.Apartments
            .FirstOrDefaultAsync(x => x.Id == new ApartmentId(responseDto!.Id));
        dbApartment.Should().NotBeNull();
        dbApartment!.Address.Should().Be(request.Address);
        dbApartment.Price.Should().Be(request.Price);
    }
}

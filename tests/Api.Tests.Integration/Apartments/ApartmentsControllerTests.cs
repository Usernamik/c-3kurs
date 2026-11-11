using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Apartments;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Apartments;
using Xunit;

namespace Api.Tests.Integration.Apartments;

public class ApartmentsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Apartment _firstTestApartment;
    private readonly Apartment _secondTestApartment;
    private readonly Apartment _thirdTestApartment;
    private readonly string _createApartmentRoute;
    private readonly string _updateApartmentRoute;

    public ApartmentsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstTestApartment = ApartmentsTestData.FirstTestApartment();
        _secondTestApartment = ApartmentsTestData.SecondTestApartment();
        _thirdTestApartment = ApartmentsTestData.ThirdTestApartment();

        _createApartmentRoute = "apartments";
        _updateApartmentRoute = $"apartments/{_secondTestApartment.Id}";
    }

    [Fact]
    public async Task ShouldCreateApartment()
    {
        // Arrange
        var request = new CreateApartmentDto(
            Address: _firstTestApartment.Address,
            Area: _firstTestApartment.Area,
            RoomsCount: _firstTestApartment.RoomsCount,
            Price: _firstTestApartment.Price,
            IsSold: _firstTestApartment.IsSold);

        // Act
        var response = await Client.PostAsJsonAsync(_createApartmentRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseDto = await response.Content.ReadFromJsonAsync<ApartmentDto>();
        responseDto.Should().NotBeNull();

        var dbApartment = await Context.Apartments
            .FirstOrDefaultAsync(x => x.Id == new ApartmentId(responseDto!.Id));
        dbApartment.Should().NotBeNull();
        dbApartment!.Address.Should().Be(_firstTestApartment.Address);
        dbApartment.Area.Should().Be(_firstTestApartment.Area);
        dbApartment.RoomsCount.Should().Be(_firstTestApartment.RoomsCount);
        dbApartment.Price.Should().Be(_firstTestApartment.Price);
        dbApartment.IsSold.Should().Be(_firstTestApartment.IsSold);
    }

    [Fact]
    public async Task ShouldNotCreateApartmentBecauseAddressAlreadyTaken()
    {
        // Arrange
        var request = new CreateApartmentDto(
            Address: _secondTestApartment.Address,
            Area: _secondTestApartment.Area,
            RoomsCount: _secondTestApartment.RoomsCount,
            Price: _secondTestApartment.Price,
            IsSold: _secondTestApartment.IsSold);

        // Act
        var response = await Client.PostAsJsonAsync(_createApartmentRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbApartment = await Context.Apartments
            .SingleAsync(x => x.Address == _secondTestApartment.Address);
        dbApartment.Id.Should().Be(_secondTestApartment.Id);
        (await Context.Apartments.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ShouldNotCreateApartmentBecauseRequestIsInvalid()
    {
        // Arrange
        var request = new CreateApartmentDto(
            Address: "AB",
            Area: 0m,
            RoomsCount: 0,
            Price: -1m);

        // Act
        var response = await Client.PostAsJsonAsync(_createApartmentRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await Context.Apartments.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ShouldUpdateApartment()
    {
        // Arrange
        var request = new UpdateApartmentDto(
            Address: _thirdTestApartment.Address,
            Area: _thirdTestApartment.Area,
            RoomsCount: _thirdTestApartment.RoomsCount,
            Price: _thirdTestApartment.Price,
            IsSold: _thirdTestApartment.IsSold);

        // Act
        var response = await Client.PutAsJsonAsync(_updateApartmentRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseDto = await response.Content.ReadFromJsonAsync<ApartmentDto>();
        responseDto.Should().NotBeNull();
        responseDto!.Id.Should().Be(_secondTestApartment.Id.Value);

        var dbApartment = await Context.Apartments
            .FirstOrDefaultAsync(x => x.Id == _secondTestApartment.Id);
        dbApartment.Should().NotBeNull();
        dbApartment!.Address.Should().Be(_thirdTestApartment.Address);
        dbApartment.Area.Should().Be(_thirdTestApartment.Area);
        dbApartment.RoomsCount.Should().Be(_thirdTestApartment.RoomsCount);
        dbApartment.Price.Should().Be(_thirdTestApartment.Price);
        dbApartment.IsSold.Should().Be(_thirdTestApartment.IsSold);
    }

    [Fact]
    public async Task ShouldNotUpdateApartmentBecauseAddressAlreadyTaken()
    {
        // Arrange
        await Context.Apartments.AddAsync(_firstTestApartment);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var request = new UpdateApartmentDto(
            Address: _firstTestApartment.Address,
            Area: _firstTestApartment.Area,
            RoomsCount: _firstTestApartment.RoomsCount,
            Price: _firstTestApartment.Price,
            IsSold: _firstTestApartment.IsSold);

        // Act
        var response = await Client.PutAsJsonAsync(_updateApartmentRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var updatedApartment = await Context.Apartments
            .SingleAsync(x => x.Id == _secondTestApartment.Id);
        updatedApartment.Address.Should().Be(_secondTestApartment.Address);

        var existingApartment = await Context.Apartments
            .SingleAsync(x => x.Id == _firstTestApartment.Id);
        existingApartment.Address.Should().Be(_firstTestApartment.Address);
    }

    [Fact]
    public async Task ShouldNotUpdateApartmentBecauseApartmentNotFound()
    {
        // Arrange
        var request = new UpdateApartmentDto(
            Address: _thirdTestApartment.Address,
            Area: _thirdTestApartment.Area,
            RoomsCount: _thirdTestApartment.RoomsCount,
            Price: _thirdTestApartment.Price,
            IsSold: _thirdTestApartment.IsSold);
        var route = $"apartments/{Guid.NewGuid()}";

        // Act
        var response = await Client.PutAsJsonAsync(route, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await Context.Apartments.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ShouldDeleteApartment()
    {
        // Act
        var response = await Client.DeleteAsync(_updateApartmentRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbApartment = await Context.Apartments
            .FirstOrDefaultAsync(x => x.Id == _secondTestApartment.Id);
        dbApartment.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteApartmentBecauseApartmentNotFound()
    {
        // Act
        var response = await Client.DeleteAsync($"apartments/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var dbApartment = await Context.Apartments
            .FirstOrDefaultAsync(x => x.Id == _secondTestApartment.Id);
        dbApartment.Should().NotBeNull();
    }

    public async Task InitializeAsync()
    {
        await Context.Apartments.AddAsync(_secondTestApartment);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Apartments.RemoveRange(Context.Apartments);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }
}

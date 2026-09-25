namespace Nurtricenter.Test.Routes.Domain;

using FluentAssertions;
using Joseco.DDD.Core.Results;
using Nurtricenter.Core.Domain.Delivery.ValueObjects;
using Nurtricenter.Core.Domain.Route;
using Nurtricenter.Core.Domain.Route.Enums;
using Nurtricenter.Core.Domain.Route.Events;

public class RouteTests
{
    // ──────────────────────────────────────────────────────────────────────
    // Constructor
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidArguments_SetsCourierIdCorrectly()
    {
        // Arrange
        var courierId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var route = new Route(Guid.NewGuid(), courierId, date);

        // Assert
        route.CourierId.Should().Be(courierId);
    }

    [Fact]
    public void Constructor_WithValidArguments_SetsScheduledDateCorrectly()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var route = new Route(Guid.NewGuid(), Guid.NewGuid(), date);

        // Assert
        route.ScheduledDate.Should().Be(date);
    }

    [Fact]
    public void Constructor_WithValidArguments_SetsStatusToPending()
    {
        // Act
        var route = new Route(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));

        // Assert
        route.Status.Should().Be(RouteStatus.Pending);
    }

    [Fact]
    public void Constructor_WithValidArguments_StartsWithNoDeliveries()
    {
        // Act
        var route = new Route(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));

        // Assert
        route.Deliveries.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithValidArguments_RaisesRouteCreatedEvent()
    {
        // Act
        var route = new Route(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));

        // Assert
        route.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RouteCreatedEvent>();
    }

    [Fact]
    public void Constructor_WithValidArguments_RaisedEventContainsCorrectData()
    {
        // Arrange
        var routeId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var route = new Route(routeId, courierId, date);

        // Assert
        var evt = route.DomainEvents.OfType<RouteCreatedEvent>().Single();
        evt.RouteId.Should().Be(routeId);
        evt.CourierId.Should().Be(courierId);
        evt.ScheduledDate.Should().Be(date);
    }

    // ──────────────────────────────────────────────────────────────────────
    // AddDelivery
    // ──────────────────────────────────────────────────────────────────────

    [Fact]
    public void AddDelivery_WithValidPackageAndAddress_IncreasesDeliveryCount()
    {
        // Arrange
        var route = CreatePendingRoute();
        var (package, address) = CreateDeliveryComponents();

        // Act
        route.AddDelivery(package, address);

        // Assert
        route.Deliveries.Should().HaveCount(1);
    }

    [Fact]
    public void AddDelivery_WithMultipleDeliveries_AddsAllToRoute()
    {
        // Arrange
        var route = CreatePendingRoute();

        // Act
        route.AddDelivery(CreateDeliveryComponents().package, CreateDeliveryComponents("PKG-001").address);
        route.AddDelivery(CreateDeliveryComponents("PKG-002").package, CreateDeliveryComponents("PKG-002").address);

        // Assert
        route.Deliveries.Should().HaveCount(2);
    }

    [Fact]
    public void AddDelivery_WithNullPackage_ThrowsDomainException()
    {
        // Arrange
        var route = CreatePendingRoute();
        var (_, address) = CreateDeliveryComponents();

        // Act
        var act = () => route.AddDelivery(null!, address);

        // Assert
        act.Should().Throw<DomainException>()
            .Where(ex => ex.Error.Code == "Route.NullPackage");
    }

    [Fact]
    public void AddDelivery_WithNullAddress_ThrowsDomainException()
    {
        // Arrange
        var route = CreatePendingRoute();
        var (package, _) = CreateDeliveryComponents();

        // Act
        var act = () => route.AddDelivery(package, null!);

        // Assert
        act.Should().Throw<DomainException>()
            .Where(ex => ex.Error.Code == "Route.NullAddress");
    }

    [Fact]
    public void AddDelivery_ToCompletedRoute_ThrowsDomainException()
    {
        // Arrange
        var route = CreateInProgressRouteWithOneDelivery();
        CompleteAllDeliveries(route);
        route.CompleteRoute();

        var (package, address) = CreateDeliveryComponents("PKG-NEW");

        // Act
        var act = () => route.AddDelivery(package, address);

        // Assert
        act.Should().Throw<DomainException>()
            .Where(ex => ex.Error.Code == "Route.CannotAddDelivery");
    }

    [Fact]
    public void AddDelivery_ToCancelledRoute_ThrowsDomainException()
    {
        // Arrange
        var route = CreatePendingRoute();
        route.CancelRoute();

        var (package, address) = CreateDeliveryComponents();

        // Act
        var act = () => route.AddDelivery(package, address);

        // Assert
        act.Should().Throw<DomainException>()
            .Where(ex => ex.Error.Code == "Route.CannotAddDelivery");
    }

    // ──────────────────────────────────────────────────────────────────────
    // Private helpers
    // ──────────────────────────────────────────────────────────────────────

    private static Route CreatePendingRoute()
        => new(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));

    private static (ValidatedPackage package, DeliveryAddress address) CreateDeliveryComponents(
        string suffix = "PKG-001")
        => (
            new ValidatedPackage(suffix, $"PAT-{suffix}", $"label-{suffix}"),
            new DeliveryAddress($"Street {suffix}", new Coordinate(1.0, 2.0))
        );

    private static Route CreateInProgressRouteWithOneDelivery()
    {
        var route = CreatePendingRoute();
        var (pkg, addr) = CreateDeliveryComponents();
        route.AddDelivery(pkg, addr);
        route.StartRoute(new Coordinate(0.0, 0.0));
        return route;
    }

    private static void CompleteAllDeliveries(Route route)
    {
        foreach (var delivery in route.Deliveries)
        {
            var confirmation = new DeliveryConfirmation(
                DateTime.UtcNow,
                "https://evidence.example.com/photo.jpg",
                "digital-signature-data");
            route.CompleteDelivery(delivery.Id, confirmation);
        }
    }
}

using FluentValidation;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nurtricenter.Application.DomainEvents;

namespace Nurtricenter.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

        services.AddValidatorsFromAssembly(
            typeof(AssemblyReference).Assembly,
            includeInternalTypes: true);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        // NOTE: MediatR's assembly scan also picks up the outbox-publishing handlers, but they
        // are REMOVED here on purpose. They depend on IExternalPublisher, which is bound only by
        // Nurtricenter.WorkerService — the single host that runs OutboxProcessor<DomainEvent> and
        // therefore the single host that ever dispatches OutboxMessage<DomainEvent>. Leaving them
        // registered in the WebApi would make DI validation fail there for handlers that can never
        // run. Hosts that own the outbox processor must call AddIntegrationMessaging instead.
        services.RemoveAll<INotificationHandler<OutboxMessage<DomainEvent>>>();

        return services;
    }

    /// <summary>
    /// Registers the outbox-to-broker publishing handlers. Call this ONLY from a host that
    /// runs the outbox processor and that has already registered an
    /// <c>Joseco.Communication.External.Contracts.Services.IExternalPublisher</c>.
    /// </summary>
    public static IServiceCollection AddIntegrationMessaging(this IServiceCollection services)
    {
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenCourierCreated>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenCourierStatusChanged>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenRouteCreated>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenCourierAssignedToRoute>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenDeliveryAddedToRoute>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenRouteStarted>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenRouteCompleted>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenRouteCancelled>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenDeliveryCompleted>();
        services.AddTransient<INotificationHandler<OutboxMessage<DomainEvent>>, PublishIntegrationEventWhenDeliveryFailed>();

        return services;
    }
}

using MediatR;

namespace SimpleAuthenticationService.Domain.Primitives;

public record DomainEvent(Guid Id) : INotification;
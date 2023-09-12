using MediatR;

namespace SimpleAuthenticationService.Domain.Abstractions;

public record DomainEvent(Guid Id) : INotification;
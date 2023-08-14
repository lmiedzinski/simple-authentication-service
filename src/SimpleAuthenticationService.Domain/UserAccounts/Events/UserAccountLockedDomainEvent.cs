using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record UserAccountLockedDomainEvent(Guid Id, UserAccountId UserAccountId) : DomainEvent(Id);
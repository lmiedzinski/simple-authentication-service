using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record UserAccountDeletedDomainEvent(Guid Id, UserAccountId UserAccountId) : DomainEvent(Id);
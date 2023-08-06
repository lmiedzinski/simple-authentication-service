using SimpleAuthenticationService.Domain.Primitives;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record UserAccountDeletedDomainEvent(Guid Id, UserAccountId UserAccountId) : DomainEvent(Id);
using SimpleAuthenticationService.Domain.Primitives;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record UserAccountUnlockedDomainEvent(Guid Id, UserAccountId UserAccountId) : DomainEvent(Id);
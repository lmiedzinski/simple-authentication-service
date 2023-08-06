using SimpleAuthenticationService.Domain.Primitives;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record UserAccountCreatedDomainEvent(Guid Id, UserAccountId UserAccountId) : DomainEvent(Id);
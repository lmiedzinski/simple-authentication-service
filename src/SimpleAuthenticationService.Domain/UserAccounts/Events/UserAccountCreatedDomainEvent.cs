using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record UserAccountCreatedDomainEvent(Guid Id, UserAccountId UserAccountId) : DomainEvent(Id);
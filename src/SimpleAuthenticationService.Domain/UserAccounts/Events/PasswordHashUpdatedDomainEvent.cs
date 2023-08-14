using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record PasswordHashUpdatedDomainEvent(Guid Id, UserAccountId UserAccountId) : DomainEvent(Id);
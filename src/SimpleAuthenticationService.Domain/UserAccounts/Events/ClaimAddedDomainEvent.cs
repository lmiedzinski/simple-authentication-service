using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record ClaimAddedDomainEvent(Guid Id, UserAccountId UserAccountId, Claim Claim) : DomainEvent(Id);
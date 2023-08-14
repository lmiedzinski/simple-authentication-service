using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record ClaimUpdatedDomainEvent(Guid Id, UserAccountId UserAccountId, ClaimId ClaimId) : DomainEvent(Id);
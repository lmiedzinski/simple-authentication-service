using SimpleAuthenticationService.Domain.Primitives;

namespace SimpleAuthenticationService.Domain.UserAccounts.Events;

public record ClaimAddedDomainEvent(Guid Id, UserAccountId UserAccountId, ClaimId ClaimId) : DomainEvent(Id);
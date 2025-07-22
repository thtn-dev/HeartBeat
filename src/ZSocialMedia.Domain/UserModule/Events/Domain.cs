using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Events;

public record class UserSuspendedEvent(Guid UserId, string Reason) : DomainEventBase;
public record class UserEmailVerifiedEvent(Guid UserId) : DomainEventBase;
public record class UserActivatedEvent(Guid UserId) : DomainEventBase;
public record class UserFollowedEvent(Guid FollowerId, Guid FollowingId) : DomainEventBase;
public record class UserUnfollowedEvent(Guid FollowerId, Guid FollowingId) : DomainEventBase;
public record class UserBlockedEvent(Guid BlockerId, Guid BlockedId) : DomainEventBase;
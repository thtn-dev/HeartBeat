using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Events;

public record UserSuspendedEvent(Guid UserId, string Reason) : DomainEventBase;
public record UserEmailVerifiedEvent(Guid UserId) : DomainEventBase;
public record UserFollowedEvent(Guid FollowerId, Guid FollowingId) : DomainEventBase;
public record UserUnfollowedEvent(Guid FollowerId, Guid FollowingId) : DomainEventBase;
public record UserBlockedEvent(Guid BlockerId, Guid BlockedId) : DomainEventBase;
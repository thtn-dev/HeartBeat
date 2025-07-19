using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Events;

public record class PostPublishedEvent(Guid PostId, Guid AuthorId) : DomainEventBase;
public record class PostDeletedEvent(Guid PostId, Guid AuthorId) : DomainEventBase;
public record class PostLikedEvent(Guid PostId, Guid UserId) : DomainEventBase;
public record class PostRepostedEvent(Guid PostId, Guid UserId) : DomainEventBase;
public record class PostViewedEvent(Guid PostId, Guid? UserId) : DomainEventBase;
public record class PostMarkedSensitiveEvent(Guid PostId) : DomainEventBase;
public record class CommentAddedEvent(Guid CommentId, Guid PostId, Guid AuthorId) : DomainEventBase;
public record class CommentEditedEvent(Guid CommentId, Guid PostId) : DomainEventBase;
public record class CommentPinnedEvent(Guid CommentId, Guid PostId) : DomainEventBase;
public record class HashtagTrendingEvent(Guid HashtagId, string Tag) : DomainEventBase;

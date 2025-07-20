using ZSocialMedia.Domain.PostModule.Common;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// Post likes/favorites
/// </summary>
public class PostLike : EntityBase<Guid>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }

    public DateTime LikedAt { get; set; }
    public LikeSource Source { get; set; } = LikeSource.Timeline;

    // Navigation
    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;

    // Index: (PostId, UserId) unique, (UserId, LikedAt), (PostId, LikedAt)
}
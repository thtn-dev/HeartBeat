using ZSocialMedia.Domain.PostModule.Common;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// Reposts/Retweets
/// </summary>
public class PostRepost : EntityBase<Guid>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }

    public DateTime RepostedAt { get; set; }
    public RepostType Type { get; set; } = RepostType.Simple;
    public string? Comment { get; set; } // For quote retweets

    // Navigation
    public virtual Post Post { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    // Index: (PostId, UserId) unique, (UserId, RepostedAt)
}
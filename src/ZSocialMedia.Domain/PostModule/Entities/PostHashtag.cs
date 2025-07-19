using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

// <summary>
/// Hashtags in posts
/// </summary>
public class PostHashtag : EntityBase<Guid>
{
    public Guid PostId { get; set; }
    public Guid HashtagId { get; set; }

    // Position in text
    public int StartPosition { get; set; }
    public int Length { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual Post Post { get; set; } = null!;
    public virtual Hashtag Hashtag { get; set; } = null!;

    // Index: (PostId, HashtagId), (HashtagId, CreatedAt)
}
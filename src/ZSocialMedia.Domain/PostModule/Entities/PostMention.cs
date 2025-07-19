using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// User mentions in posts
/// </summary>
public class PostMention : EntityBase<Guid>
{
    public Guid PostId { get; set; }
    public Guid MentionedUserId { get; set; }

    // Mention details
    public int StartPosition { get; set; } // Position in text
    public int Length { get; set; } // Length of mention
    public bool IsNotified { get; set; }
    public DateTime? NotifiedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual Post Post { get; set; } = null!;
    public virtual User MentionedUser { get; set; } = null!;

    // Index: (PostId, MentionedUserId), (MentionedUserId, CreatedAt)
}
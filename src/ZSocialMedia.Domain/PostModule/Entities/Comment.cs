using ZSocialMedia.Domain.PostModule.Common;
using ZSocialMedia.Domain.PostModule.Events;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// Comments/Replies on posts
/// </summary>
public class Comment : EntityAggregateBase<Guid>, IAuditableEntity, ISoftDelete
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; } // Author
    public Guid? ParentCommentId { get; set; } // For nested comments

    // Content
    public required string Content { get; set; } // Max 280 chars
    public ContentFormat Format { get; set; } = ContentFormat.PlainText;

    // Thread info
    public int Depth { get; set; } // Nesting level
    public string? ThreadPath { get; set; } // For efficient queries

    // Metrics
    public int LikesCount { get; set; }
    public int RepliesCount { get; set; }

    // Status
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsPinned { get; set; } // Pinned by post-author
    public bool IsHighlighted { get; set; } // Highlighted by author

    // Moderation
    public bool IsHidden { get; set; }
    public string? HiddenReason { get; set; }
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public virtual Post Post { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public virtual ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();

    // Domain methods
    public void Edit(string newContent)
    {
        Content = newContent;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RegisterDomainEvent(new CommentEditedEvent(Id, PostId));
    }

    public void Pin()
    {
        IsPinned = true;
        RegisterDomainEvent(new CommentPinnedEvent(Id, PostId));
    }
}

/// <summary>
/// Comment likes
/// </summary>
public class CommentLike : EntityBase<Guid>
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }

    public DateTime LikedAt { get; set; }

    // Navigation
    public virtual Comment Comment { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    // Index: (CommentId, UserId) unique, (UserId, LikedAt)
}
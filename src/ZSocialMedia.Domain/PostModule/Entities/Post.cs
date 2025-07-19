using System.Xml.Linq;
using ZSocialMedia.Domain.PostModule.Common;
using ZSocialMedia.Domain.PostModule.Events;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// Main post entity - the core content type
/// </summary>
public class Post : EntityAggregateBase<Guid>, IAuditableEntity, ISoftDelete
{
    public Guid UserId { get; set; } // Author

    // Content
    public required string Content { get; set; } // Max 280 chars for normal users
    public PostType Type { get; set; } = PostType.Regular;
    public ContentFormat Format { get; set; } = ContentFormat.PlainText;

    // Reply/Thread
    public Guid? ParentPostId { get; set; } // If this is a reply
    public Guid? ThreadRootId { get; set; } // Original post in thread
    public int ReplyDepth { get; set; } = 0; // How deep in thread
    public Guid? QuotedPostId { get; set; } // For quote tweets

    // Visibility & Privacy
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public bool IsCommentsEnabled { get; set; } = true;
    public CommentPermission CommentPermission { get; set; } = CommentPermission.Everyone;

    // Status
    public PostStatus Status { get; set; } = PostStatus.Published;
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool IsPinned { get; set; }
    public DateTime? PinnedAt { get; set; }

    // Metrics (denormalized for performance)
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public int RepostsCount { get; set; }
    public int QuotesCount { get; set; }
    public int ViewsCount { get; set; }
    public int SharesCount { get; set; }
    public int BookmarksCount { get; set; }

    // Content flags
    public bool HasMedia { get; set; }
    public bool HasLinks { get; set; }
    public bool HasMentions { get; set; }
    public bool HasHashtags { get; set; }
    public bool HasPoll { get; set; }

    // Moderation
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }
    public bool IsSensitive { get; set; } // User marked
    public bool IsNSFW { get; set; } // System detected
    public ContentRating ContentRating { get; set; } = ContentRating.General;

    // SEO & Discovery
    public string? Slug { get; set; } // For URL sharing
    public string? PreviewText { get; set; } // First 100 chars for preview

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation properties
    public virtual User Author { get; set; } = null!;
    public virtual Post? ParentPost { get; set; }
    public virtual Post? QuotedPost { get; set; }
    public virtual ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    public virtual ICollection<PostMention> Mentions { get; set; } = new List<PostMention>();
    public virtual ICollection<PostHashtag> Hashtags { get; set; } = new List<PostHashtag>();
    public virtual ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    public virtual ICollection<PostBookmark> Bookmarks { get; set; } = new List<PostBookmark>();
    public virtual ICollection<PostRepost> Reposts { get; set; } = new List<PostRepost>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    // Domain methods
    public void Publish()
    {
        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
        RegisterDomainEvent(new PostPublishedEvent(Id, UserId));
    }

    public void Pin()
    {
        IsPinned = true;
        PinnedAt = DateTime.UtcNow;
    }

    public void MarkAsSensitive()
    {
        IsSensitive = true;
        RegisterDomainEvent(new PostMarkedSensitiveEvent(Id));
    }

    public void IncrementViews()
    {
        ViewsCount++;
        // Don't raise event for every view - batch process
    }
}
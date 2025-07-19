using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// Saved posts/bookmarks
/// </summary>
public class PostBookmark : EntityBase<Guid>, IAuditableEntity
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }

    // Organization
    public string? FolderName { get; set; } // User-defined folders
    public string? Note { get; set; } // Personal note
    public string? Tags { get; set; } // JSON array of user tags

    public DateTime BookmarkedAt { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual Post Post { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    // Index: (UserId, PostId) unique, (UserId, BookmarkedAt), (UserId, FolderName)
}
using ZSocialMedia.Domain.PostModule.Common;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// Media attachments for posts
/// </summary>
public class PostMedia : EntityBase<Guid>, IAuditableEntity
{
    public Guid PostId { get; set; }

    // Media info
    public required string MediaUrl { get; set; }
    public required string ThumbnailUrl { get; set; }
    public MediaType MediaType { get; set; }
    public required string MimeType { get; set; }
    public long FileSize { get; set; } // In bytes
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; } // For video/audio in seconds

    // Processing
    public MediaProcessingStatus ProcessingStatus { get; set; } = MediaProcessingStatus.Pending;
    public string? ProcessingError { get; set; }
    public DateTime? ProcessedAt { get; set; }

    // Metadata
    public string? AltText { get; set; } // Accessibility
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public string? BlurHash { get; set; } // For progressive loading

    // Moderation
    public bool IsNSFW { get; set; }
    public float? NSFWScore { get; set; } // AI detection score
    public bool IsReviewed { get; set; }

    // Storage
    public string? StorageKey { get; set; } // S3 key
    public string? CDNUrl { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual Post Post { get; set; } = null!;
}
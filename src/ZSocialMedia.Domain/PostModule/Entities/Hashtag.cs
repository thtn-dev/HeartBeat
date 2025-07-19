using ZSocialMedia.Domain.PostModule.Common;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;
/// <summary>
/// Master hashtag list
/// </summary>
public class Hashtag : EntityBase<Guid>, IAuditableEntity
{
    public required string Tag { get; set; } // Without # symbol, lowercase
    public required string DisplayTag { get; set; } // Original case

    // Metrics
    public long TotalUsageCount { get; set; }
    public long DailyUsageCount { get; set; }
    public long WeeklyUsageCount { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime? TrendingStartedAt { get; set; }
    public bool IsTrending { get; set; }
    public int TrendingRank { get; set; }

    // Categorization
    public HashtagCategory Category { get; set; } = HashtagCategory.General;
    public string? Description { get; set; }
    public string? RelatedTags { get; set; } // JSON array

    // Moderation
    public bool IsBanned { get; set; }
    public string? BannedReason { get; set; }
    public bool IsPromoted { get; set; } // For ads/campaigns
    public DateTime? PromotedUntil { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();

    // Index: Tag (unique), TrendingRank, Category

    public void Normalize()
    {
        Tag = Tag.Trim().ToLowerInvariant();
        DisplayTag = DisplayTag.Trim();
        Description = Description?.Trim();
        RelatedTags = RelatedTags?.Trim();
    }
}
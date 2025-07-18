using ZSocialMedia.Domain.UserModule.Common.Enums;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

/// <summary>
/// User preferences and settings
/// </summary>
public class UserSettings : EntityBase<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; }

    // Privacy Settings
    public bool IsPrivateAccount { get; set; }
    public bool AllowDirectMessages { get; set; } = true;
    public DirectMessageFilter DirectMessageFilter { get; set; } = DirectMessageFilter.Everyone;
    public bool ShowOnlineStatus { get; set; } = true;
    public bool ShowReadReceipts { get; set; } = true;

    // Content Settings  
    public Language PreferredLanguage { get; set; } = Language.English;
    public string Timezone { get; set; } = "UTC";
    public bool ShowSensitiveContent { get; set; }
    public ContentQualityFilter QualityFilter { get; set; } = ContentQualityFilter.Standard;

    // Notification Preferences
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;

    // Feed Preferences
    public FeedSortOrder DefaultFeedSort { get; set; } = FeedSortOrder.Chronological;
    public bool ShowReposts { get; set; } = true;
    public bool ShowReplies { get; set; } = true;
    public bool AutoplayVideos { get; set; } = true;

    // Data & Privacy
    public bool AllowAnalytics { get; set; } = true;
    public bool AllowPersonalization { get; set; } = true;
    public DataDownloadFrequency DataDownloadFrequency { get; set; } = DataDownloadFrequency.Never;

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
}
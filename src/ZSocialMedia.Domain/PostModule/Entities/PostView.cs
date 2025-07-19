using ZSocialMedia.Domain.PostModule.Common;
using ZSocialMedia.Domain.UserModule.Common.Enums;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.PostModule.Entities;

/// <summary>
/// Post view tracking - can be batched
/// </summary>
public class PostView : EntityBase<Guid>
{
    public Guid PostId { get; set; }
    public Guid? UserId { get; set; } // Null for anonymous

    // View details
    public DateTime ViewedAt { get; set; }
    public int DurationSeconds { get; set; } // Time spent
    public ViewSource Source { get; set; }
    public string? SessionId { get; set; }
    public string? IpAddressHash { get; set; } // Hashed for privacy
    public string? Country { get; set; }

    // Device info
    public DeviceType DeviceType { get; set; }
    public string? UserAgent { get; set; }

    // Navigation
    public virtual Post Post { get; set; } = null!;
    public virtual User? User { get; set; }

    // Index: (PostId, ViewedAt), (UserId, ViewedAt)
    // Consider partitioning by date
}
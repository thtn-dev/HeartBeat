using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

// <summary>
/// Follower/Following relationship
/// </summary>
public class UserFollower : EntityBase<Guid>, IAuditableEntity
{
    public Guid FollowerId { get; set; } // The user who follows
    public Guid FollowingId { get; set; } // The user being followed

    // Relationship metadata
    public DateTime FollowedAt { get; set; }
    public bool IsNotificationEnabled { get; set; } = true; // Get notified of new posts
    public bool IsCloseFriend { get; set; } // For close friends features
    public bool IsPending { get; set; } // For private accounts
    public DateTime? AcceptedAt { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual User Follower { get; set; } = null!;
    public virtual User Following { get; set; } = null!;

}
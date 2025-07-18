using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

/// <summary>
/// Extended profile information - separated for performance
/// </summary>
public class UserProfile : EntityBase<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; } // Reference to User

    // Profile Info
    public required string DisplayName { get; set; }
    public string? Bio { get; set; } // Max 160 chars like Twitter
    public string? Location { get; set; }
    public string? Website { get; set; }
    public DateTime? BirthDate { get; set; }

    // Media
    public string? AvatarUrl { get; set; }
    public string? CoverImageUrl { get; set; }

    // Stats (denormalized for performance)
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
    public int LikesCount { get; set; }


    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
}

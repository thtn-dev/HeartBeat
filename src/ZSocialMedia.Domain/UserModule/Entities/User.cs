using ZSocialMedia.Domain.UserModule.Events;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

// <summary>
/// Core user entity - contains authentication and basic info
/// </summary>
public class User : EntityAggregateBase<Guid>, IAuditableEntity, ISoftDelete
{
    // Basic Info
    public required string Username { get; set; } // Unique, used for @mentions
    public required string Email { get; set; } // Unique, for login
    public required string PasswordHash { get; set; }

    // Account Status
    public bool IsEmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSuspended { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }

    // Account Type
    public bool IsVerified { get; set; } // Blue checkmark
    public DateTime? VerifiedAt { get; set; }

    // Security
    public bool IsTwoFactorEnabled { get; set; }
    public string? TwoFactorSecretKey { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEndAt { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation properties (no FK constraints)
    public virtual UserCredential? Credential { get; set; }
    public virtual UserProfile? Profile { get; set; }
    public virtual UserSettings? Settings { get; set; }
    public virtual ICollection<UserFollower> Followers { get; set; } = [];
    public virtual ICollection<UserFollower> Following { get; set; } = [];
    public virtual ICollection<UserBlock> BlockedUsers { get; set; } = [];
    public virtual ICollection<UserBlock> BlockedByUsers { get; set; } = [];
    public virtual ICollection<UserRole> Roles { get; set; } = [];

    // Domain methods
    public void Suspend(string reason)
    {
        IsSuspended = true;
        SuspendedAt = DateTime.UtcNow;
        SuspensionReason = reason;
        // Raise domain event
        RegisterDomainEvent(new UserSuspendedEvent(Id, reason));
    }

    public void Activate()
    {
        IsSuspended = false;
        SuspendedAt = null;
        SuspensionReason = null;
        IsActive = true;
        // Raise domain event
        RegisterDomainEvent(new UserActivatedEvent(Id));
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        EmailVerifiedAt = DateTime.UtcNow;
        RegisterDomainEvent(new UserEmailVerifiedEvent(Id));
    }
}
using ZSocialMedia.Domain.UserModule.Common.Enums;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

// <summary>
/// Email verification tokens
/// </summary>
public class EmailVerification : EntityBase<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; }

    // Email info
    public required string Email { get; set; }
    public required string Token { get; set; } // Hashed
    public required string TokenHash { get; set; } // For lookup
    public VerificationType Type { get; set; }

    // Status
    public DateTime ExpiresAt { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedByIp { get; set; }

    // Attempt tracking
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;

    // Methods
    public bool IsValid => !IsVerified && DateTime.UtcNow < ExpiresAt;

    public void Verify(string ip)
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        VerifiedByIp = ip;
    }

    public void RecordAttempt()
    {
        AttemptCount++;
        LastAttemptAt = DateTime.UtcNow;
    }
}

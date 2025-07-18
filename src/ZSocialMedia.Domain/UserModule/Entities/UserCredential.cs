using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

/// <summary>
/// Separated credentials for security - easier to encrypt/protect
/// </summary>
public class UserCredential : EntityBase<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; }

    // Password
    public required string PasswordHash { get; set; }
    public string? PasswordSalt { get; set; } // If not using BCrypt
    public DateTime PasswordChangedAt { get; set; }
    public DateTime? PasswordExpiresAt { get; set; }

    // Security
    public int FailedLoginAttempts { get; set; }
    public DateTime? LastFailedLoginAt { get; set; }
    public DateTime? LockoutEndAt { get; set; }
    public bool RequirePasswordChange { get; set; }

    // Login tracking
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public string? LastLoginUserAgent { get; set; }
    public string? LastLoginLocation { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;

    // Methods
    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        LastFailedLoginAt = DateTime.UtcNow;

        // Lock after 5 attempts
        if (FailedLoginAttempts >= 5)
        {
            LockoutEndAt = DateTime.UtcNow.AddMinutes(30);
        }
    }

    public void RecordSuccessfulLogin(string ipAddress, string userAgent)
    {
        FailedLoginAttempts = 0;
        LastFailedLoginAt = null;
        LockoutEndAt = null;
        LastLoginAt = DateTime.UtcNow;
        LastLoginIp = ipAddress;
        LastLoginUserAgent = userAgent;
    }

    public bool IsLocked => LockoutEndAt.HasValue && LockoutEndAt > DateTime.UtcNow;
}
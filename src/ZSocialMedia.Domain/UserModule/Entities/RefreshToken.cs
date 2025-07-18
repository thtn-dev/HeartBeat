using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

/// <summary>
/// JWT Refresh tokens for token rotation
/// </summary>
public class RefreshToken : EntityBase<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; }
    public Guid? SessionId { get; set; }
    // Token data
    public required string Token { get; set; }
    public required string JwtId { get; set; } // JTI claim from access token
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }

    // Token metadata
    public required string CreatedByIp { get; set; }
    public string? UsedByIp { get; set; }
    public string? UserAgent { get; set; }

    // Token family for rotation
    public string? ReplacedByToken { get; set; }
    public string? PreviousToken { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual UserSession? Session { get; set; }

    // Methods
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsUsed && !IsRevoked && !IsExpired;

    public void Revoke(string reason)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
    }

    public void MarkAsUsed(string byIp)
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UsedByIp = byIp;
    }
}

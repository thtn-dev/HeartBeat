using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSocialMedia.Domain.UserModule.Common.Enums;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

/// <summary>
/// Active user sessions for multi-device support
/// </summary>
public class UserSession : EntityBase<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; }

    // Session info
    public required string SessionToken { get; set; } // Unique session identifier
    public required string DeviceId { get; set; }
    public required string DeviceName { get; set; }
    public DeviceType DeviceType { get; set; }
    public string? DeviceModel { get; set; }
    public required string IpAddress { get; set; }
    public string? Location { get; set; } // City, Country
    public required string UserAgent { get; set; }

    // Session state
    public DateTime LoginAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public SessionEndReason? EndReason { get; set; }

    // Security
    public bool IsTrustedDevice { get; set; }
    public string? FcmToken { get; set; } // For push notifications

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;

    // Methods
    public void End(SessionEndReason reason)
    {
        IsActive = false;
        EndReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateActivity()
    {
        LastActivityAt = DateTime.UtcNow;
        // Extend expiration on activity
        if (ExpiresAt < DateTime.UtcNow.AddDays(7))
        {
            ExpiresAt = DateTime.UtcNow.AddDays(30);
        }
    }
}
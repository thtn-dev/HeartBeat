using ZSocialMedia.Domain.UserModule.Common.Enums;
using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

/// <summary>
/// Block relationship between users
/// </summary>
public class UserBlock : EntityBase<Guid>, IAuditableEntity
{
    public Guid BlockerId { get; set; } // User who blocks
    public Guid BlockedId { get; set; } // User being blocked

    // Block metadata
    public DateTime BlockedAt { get; set; }
    public BlockReason Reason { get; set; } = BlockReason.Other;
    public string? ReasonDetails { get; set; }

    // Audit fields  
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual User Blocker { get; set; } = null!;
    public virtual User Blocked { get; set; } = null!;

}
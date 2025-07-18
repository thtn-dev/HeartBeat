using ZSocialMedia.Shared;

namespace ZSocialMedia.Domain.UserModule.Entities;

public class Role : EntityBase<Guid>, IAuditableEntity
{
    public required string Name { get; set; } // Unique role name
    public required string Description { get; set; } // Role description
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}

public class UserRole : EntityBase<Guid>
{
    public Guid UserId { get; set; } // FK to User
    public Guid RoleId { get; set; } // FK to Role

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
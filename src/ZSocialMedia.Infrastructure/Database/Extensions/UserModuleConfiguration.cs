using Microsoft.EntityFrameworkCore;
using ZSocialMedia.Domain.UserModule.Entities;

namespace ZSocialMedia.Infrastructure.Database.Extensions;

public static class UserModuleConfiguration
{
    internal static void ConfigureUser(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "user");
            
            // Primary Key
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            // Optional fields
            entity.Property(x => x.SuspensionReason).HasMaxLength(500);
            entity.Property(x => x.LastLoginIp).HasMaxLength(45); // IPv6 max length
            entity.Property(x => x.TwoFactorSecretKey).HasMaxLength(255);

            // Indexes
            entity.HasIndex(x => x.Username).IsUnique().HasDatabaseName("IX_Users_Username");
            entity.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Users_Email");
            entity.HasIndex(x => x.IsActive).HasDatabaseName("IX_Users_IsActive");
            entity.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_Users_CreatedAt");
            entity.HasIndex(x => x.IsDeleted).HasDatabaseName("IX_Users_IsDeleted");

            // Audit configuration
            entity.ConfigureAuditFields();
            entity.ConfigureSoftDelete();

            // Self-referencing relationships through junction tables
            entity.HasMany(x => x.Followers)
                .WithOne(x => x.Following)
                .HasForeignKey(x => x.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Following)
                .WithOne(x => x.Follower)
                .HasForeignKey(x => x.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.BlockedUsers)
                .WithOne(x => x.Blocker)
                .HasForeignKey(x => x.BlockerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.BlockedByUsers)
                .WithOne(x => x.Blocked)
                .HasForeignKey(x => x.BlockedId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-one relationships
            entity.HasOne(x => x.Credential)
                .WithOne(x => x.User)
                .HasForeignKey<UserCredential>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Profile)
                .WithOne(x => x.User)
                .HasForeignKey<UserProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Settings)
                .WithOne(x => x.User)
                .HasForeignKey<UserSettings>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    internal static void ConfigureUserCredential(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.ToTable("UserCredentials", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            // Optional fields
            entity.Property(x => x.PasswordSalt).HasMaxLength(255);
            entity.Property(x => x.LastLoginIp).HasMaxLength(45);
            entity.Property(x => x.LastLoginUserAgent).HasMaxLength(512);
            entity.Property(x => x.LastLoginLocation).HasMaxLength(255);

            // Indexes
            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("IX_UserCredentials_UserId");
            entity.HasIndex(x => x.LockoutEndAt).HasDatabaseName("IX_UserCredentials_LockoutEndAt");

            entity.ConfigureAuditFields();
        });
    }

    internal static void ConfigureUserProfile(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfiles", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            // Optional fields with length limits
            entity.Property(x => x.Bio).HasMaxLength(160); // Twitter-like limit
            entity.Property(x => x.Location).HasMaxLength(100);
            entity.Property(x => x.Website).HasMaxLength(255);
            entity.Property(x => x.AvatarUrl).HasMaxLength(512);
            entity.Property(x => x.CoverImageUrl).HasMaxLength(512);

            // Indexes
            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("IX_UserProfiles_UserId");
            entity.HasIndex(x => x.DisplayName).HasDatabaseName("IX_UserProfiles_DisplayName");

            entity.ConfigureAuditFields();
        });
    }

    internal static void ConfigureUserSettings(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.ToTable("UserSettings", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Enum conversions
            entity.Property(x => x.DirectMessageFilter)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.PreferredLanguage)
                .HasConversion<string>()
                .HasMaxLength(10);

            entity.Property(x => x.QualityFilter)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.DefaultFeedSort)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.DataDownloadFrequency)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Timezone).HasMaxLength(100);

            // Indexes
            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("IX_UserSettings_UserId");

            entity.ConfigureAuditFields();
        });
    }

    internal static void ConfigureUserFollower(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserFollower>(entity =>
        {
            entity.ToTable("UserFollowers", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Composite unique constraint
            entity.HasIndex(x => new { x.FollowerId, x.FollowingId })
                .IsUnique()
                .HasDatabaseName("IX_UserFollowers_FollowerFollowing");

            // Performance indexes
            entity.HasIndex(x => x.FollowerId).HasDatabaseName("IX_UserFollowers_FollowerId");
            entity.HasIndex(x => x.FollowingId).HasDatabaseName("IX_UserFollowers_FollowingId");
            entity.HasIndex(x => x.FollowedAt).HasDatabaseName("IX_UserFollowers_FollowedAt");
            entity.HasIndex(x => x.IsPending).HasDatabaseName("IX_UserFollowers_IsPending");

            entity.ConfigureAuditFields();

            // Check constraint to prevent self-following
            entity.HasCheckConstraint("CK_UserFollowers_NoSelfFollow", 
                "\"FollowerId\" != \"FollowingId\"");
        });
    }

    internal static void ConfigureUserBlock(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserBlock>(entity =>
        {
            entity.ToTable("UserBlocks", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Enum conversion
            entity.Property(x => x.Reason)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.ReasonDetails).HasMaxLength(500);

            // Composite unique constraint
            entity.HasIndex(x => new { x.BlockerId, x.BlockedId })
                .IsUnique()
                .HasDatabaseName("IX_UserBlocks_BlockerBlocked");

            // Performance indexes
            entity.HasIndex(x => x.BlockerId).HasDatabaseName("IX_UserBlocks_BlockerId");
            entity.HasIndex(x => x.BlockedId).HasDatabaseName("IX_UserBlocks_BlockedId");
            entity.HasIndex(x => x.BlockedAt).HasDatabaseName("IX_UserBlocks_BlockedAt");

            entity.ConfigureAuditFields();

            // Check constraint to prevent self-blocking
            entity.HasCheckConstraint("CK_UserBlocks_NoSelfBlock", 
                "\"BlockerId\" != \"BlockedId\"");
        });
    }

    internal static void ConfigureUserSession(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.ToTable("UserSessions", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.SessionToken)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.DeviceName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            entity.Property(x => x.UserAgent)
                .IsRequired()
                .HasMaxLength(512);

            // Optional fields
            entity.Property(x => x.DeviceModel).HasMaxLength(255);
            entity.Property(x => x.Location).HasMaxLength(255);
            entity.Property(x => x.FcmToken).HasMaxLength(255);

            // Enum conversions
            entity.Property(x => x.DeviceType)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.EndReason)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            entity.HasIndex(x => x.SessionToken).IsUnique().HasDatabaseName("IX_UserSessions_SessionToken");
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_UserSessions_UserId");
            entity.HasIndex(x => x.IsActive).HasDatabaseName("IX_UserSessions_IsActive");
            entity.HasIndex(x => x.ExpiresAt).HasDatabaseName("IX_UserSessions_ExpiresAt");
            entity.HasIndex(x => x.LastActivityAt).HasDatabaseName("IX_UserSessions_LastActivityAt");

            entity.ConfigureAuditFields();

            // Relationship
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    internal static void ConfigureRefreshToken(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.JwtId)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.CreatedByIp)
                .IsRequired()
                .HasMaxLength(45);

            // Optional fields
            entity.Property(x => x.UsedByIp).HasMaxLength(45);
            entity.Property(x => x.UserAgent).HasMaxLength(512);
            entity.Property(x => x.ReplacedByToken).HasMaxLength(255);
            entity.Property(x => x.PreviousToken).HasMaxLength(255);
            entity.Property(x => x.RevokedReason).HasMaxLength(255);

            // Indexes
            entity.HasIndex(x => x.Token).IsUnique().HasDatabaseName("IX_RefreshTokens_Token");
            entity.HasIndex(x => x.JwtId).HasDatabaseName("IX_RefreshTokens_JwtId");
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_RefreshTokens_UserId");
            entity.HasIndex(x => x.ExpiresAt).HasDatabaseName("IX_RefreshTokens_ExpiresAt");
            entity.HasIndex(x => x.IsUsed).HasDatabaseName("IX_RefreshTokens_IsUsed");
            entity.HasIndex(x => x.IsRevoked).HasDatabaseName("IX_RefreshTokens_IsRevoked");

            entity.ConfigureAuditFields();

            // Relationships
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Session)
                .WithMany()
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    internal static void ConfigureEmailVerification(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.ToTable("EmailVerifications", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(255);

            // Optional fields
            entity.Property(x => x.VerifiedByIp).HasMaxLength(45);

            // Enum conversion
            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            entity.HasIndex(x => x.TokenHash).HasDatabaseName("IX_EmailVerifications_TokenHash");
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_EmailVerifications_UserId");
            entity.HasIndex(x => x.Email).HasDatabaseName("IX_EmailVerifications_Email");
            entity.HasIndex(x => x.ExpiresAt).HasDatabaseName("IX_EmailVerifications_ExpiresAt");
            entity.HasIndex(x => x.IsVerified).HasDatabaseName("IX_EmailVerifications_IsVerified");

            entity.ConfigureAuditFields();

            // Relationship
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    internal static void ConfigureRole(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            // Indexes
            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("IX_Roles_Name");

            entity.ConfigureAuditFields();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            // Composite unique constraint
            entity.HasIndex(x => new { x.UserId, x.RoleId })
                .IsUnique()
                .HasDatabaseName("IX_UserRoles_UserRole");

            // Performance indexes
            entity.HasIndex(x => x.UserId).HasDatabaseName("IX_UserRoles_UserId");
            entity.HasIndex(x => x.RoleId).HasDatabaseName("IX_UserRoles_RoleId");

            // Relationships
            entity.HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // Helper extension methods for common configurations
    private static void ConfigureAuditFields<T>(this Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> entity) 
        where T : class
    {
        entity.Property("CreatedAt").IsRequired();
        entity.Property("CreatedBy").HasMaxLength(255);
        entity.Property("UpdatedAt");
        entity.Property("UpdatedBy").HasMaxLength(255);
    }

    private static void ConfigureSoftDelete<T>(this Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> entity) 
        where T : class
    {
        entity.Property("IsDeleted").HasDefaultValue(false);
        entity.Property("DeletedAt");
        entity.Property("DeletedBy").HasMaxLength(255);
        
        // Global query filter for soft delete
        // TODO: Implement global query filter in DbContext
        // entity.HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
    }
}
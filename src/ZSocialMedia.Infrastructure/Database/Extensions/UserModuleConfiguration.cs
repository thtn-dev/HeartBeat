using Microsoft.EntityFrameworkCore;
using ZSocialMedia.Domain.UserModule.Entities;

namespace ZSocialMedia.Infrastructure.Database.Extensions;

public static class UserModuleConfiguration
{
    internal static void ConfigureUser(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "user");
            
            // Primary Key
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Username)
                .HasColumnName("username")
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(x => x.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            // Optional fields
            entity.Property(x => x.SuspensionReason).HasColumnName("suspension_reason").HasMaxLength(500);
            entity.Property(x => x.LastLoginIp).HasColumnName("last_login_ip").HasMaxLength(45); // IPv6 max length
            entity.Property(x => x.TwoFactorSecretKey).HasColumnName("two_factor_secret_key").HasMaxLength(255);

            // Indexes
            entity.HasIndex(x => x.Username).IsUnique().HasDatabaseName("ix_users_username");
            entity.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ix_users_email");
            entity.HasIndex(x => x.IsActive).HasDatabaseName("ix_users_is_active");
            entity.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_users_created_at");
            entity.HasIndex(x => x.IsDeleted).HasDatabaseName("ix_users_is_deleted");

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
            entity.ToTable("user_credentials", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            // Optional fields
            entity.Property(x => x.PasswordSalt).HasColumnName("password_salt").HasMaxLength(255);
            entity.Property(x => x.LastLoginIp).HasColumnName("last_login_ip").HasMaxLength(45);
            entity.Property(x => x.LastLoginUserAgent).HasColumnName("last_login_user_agent").HasMaxLength(512);
            entity.Property(x => x.LastLoginLocation).HasColumnName("last_login_location").HasMaxLength(255);

            // Indexes
            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("ix_user_credentials_user_id");
            entity.HasIndex(x => x.LockoutEndAt).HasDatabaseName("ix_user_credentials_lockout_end_at");

            entity.ConfigureAuditFields();
        });
    }

    internal static void ConfigureUserProfile(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.DisplayName)
                .HasColumnName("display_name")
                .IsRequired()
                .HasMaxLength(100);

            // Optional fields with length limits
            entity.Property(x => x.Bio).HasColumnName("bio").HasMaxLength(160); // Twitter-like limit
            entity.Property(x => x.Location).HasColumnName("location").HasMaxLength(100);
            entity.Property(x => x.Website).HasColumnName("website").HasMaxLength(255);
            entity.Property(x => x.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(512);
            entity.Property(x => x.CoverImageUrl).HasColumnName("cover_image_url").HasMaxLength(512);

            // Indexes
            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("ix_user_profiles_user_id");
            entity.HasIndex(x => x.DisplayName).HasDatabaseName("ix_user_profiles_display_name");

            entity.ConfigureAuditFields();
        });
    }

    internal static void ConfigureUserSettings(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.ToTable("user_settings", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Enum conversions
            entity.Property(x => x.DirectMessageFilter)
                .HasColumnName("direct_message_filter")
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.PreferredLanguage)
                .HasColumnName("preferred_language")
                .HasConversion<string>()
                .HasMaxLength(10);

            entity.Property(x => x.QualityFilter)
                .HasColumnName("quality_filter")
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.DefaultFeedSort)
                .HasColumnName("default_feed_sort")
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.DataDownloadFrequency)
                .HasColumnName("data_download_frequency")
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Timezone).HasColumnName("timezone").HasMaxLength(100);

            // Indexes
            entity.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("ix_user_settings_user_id");

            entity.ConfigureAuditFields();
        });
    }

    internal static void ConfigureUserFollower(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserFollower>(entity =>
        {
            entity.ToTable("user_followers", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Composite unique constraint
            entity.HasIndex(x => new { x.FollowerId, x.FollowingId })
                .IsUnique()
                .HasDatabaseName("ix_user_followers_follower_following");

            // Performance indexes
            entity.HasIndex(x => x.FollowerId).HasDatabaseName("ix_user_followers_follower_id");
            entity.HasIndex(x => x.FollowingId).HasDatabaseName("ix_user_followers_following_id");
            entity.HasIndex(x => x.FollowedAt).HasDatabaseName("ix_user_followers_followed_at");
            entity.HasIndex(x => x.IsPending).HasDatabaseName("ix_user_followers_is_pending");

            entity.ConfigureAuditFields();

            // Check constraint to prevent self-following
            entity.HasCheckConstraint("ck_user_followers_no_self_follow", 
                "\"follower_id\" != \"following_id\"");
        });
    }

    internal static void ConfigureUserBlock(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserBlock>(entity =>
        {
            entity.ToTable("user_blocks", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Enum conversion
            entity.Property(x => x.Reason)
                .HasColumnName("reason")
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.ReasonDetails).HasColumnName("reason_details").HasMaxLength(500);

            // Composite unique constraint
            entity.HasIndex(x => new { x.BlockerId, x.BlockedId })
                .IsUnique()
                .HasDatabaseName("ix_user_blocks_blocker_blocked");

            // Performance indexes
            entity.HasIndex(x => x.BlockerId).HasDatabaseName("ix_user_blocks_blocker_id");
            entity.HasIndex(x => x.BlockedId).HasDatabaseName("ix_user_blocks_blocked_id");
            entity.HasIndex(x => x.BlockedAt).HasDatabaseName("ix_user_blocks_blocked_at");

            entity.ConfigureAuditFields();

            // Check constraint to prevent self-blocking
            entity.HasCheckConstraint("ck_user_blocks_no_self_block", 
                "\"blocker_id\" != \"blocked_id\"");
        });
    }

    internal static void ConfigureUserSession(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.ToTable("user_sessions", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.SessionToken)
                .HasColumnName("session_token")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.DeviceId)
                .HasColumnName("device_id")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.DeviceName)
                .HasColumnName("device_name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.IpAddress)
                .HasColumnName("ip_address")
                .IsRequired()
                .HasMaxLength(45);

            entity.Property(x => x.UserAgent)
                .HasColumnName("user_agent")
                .IsRequired()
                .HasMaxLength(512);

            // Optional fields
            entity.Property(x => x.DeviceModel).HasColumnName("device_model").HasMaxLength(255);
            entity.Property(x => x.Location).HasColumnName("location").HasMaxLength(255);
            entity.Property(x => x.FcmToken).HasColumnName("fcm_token").HasMaxLength(255);

            // Enum conversions
            entity.Property(x => x.DeviceType)
                .HasColumnName("device_type")
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.EndReason)
                .HasColumnName("end_reason")
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            entity.HasIndex(x => x.SessionToken).IsUnique().HasDatabaseName("ix_user_sessions_session_token");
            entity.HasIndex(x => x.UserId).HasDatabaseName("ix_user_sessions_user_id");
            entity.HasIndex(x => x.IsActive).HasDatabaseName("ix_user_sessions_is_active");
            entity.HasIndex(x => x.ExpiresAt).HasDatabaseName("ix_user_sessions_expires_at");
            entity.HasIndex(x => x.LastActivityAt).HasDatabaseName("ix_user_sessions_last_activity_at");

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
            entity.ToTable("refresh_tokens", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Token)
                .HasColumnName("token")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.JwtId)
                .HasColumnName("jwt_id")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.CreatedByIp)
                .HasColumnName("created_by_ip")
                .IsRequired()
                .HasMaxLength(45);

            // Optional fields
            entity.Property(x => x.UsedByIp).HasColumnName("used_by_ip").HasMaxLength(45);
            entity.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(512);
            entity.Property(x => x.ReplacedByToken).HasColumnName("replaced_by_token").HasMaxLength(255);
            entity.Property(x => x.PreviousToken).HasColumnName("previous_token").HasMaxLength(255);
            entity.Property(x => x.RevokedReason).HasColumnName("revoked_reason").HasMaxLength(255);

            // Indexes
            entity.HasIndex(x => x.Token).IsUnique().HasDatabaseName("ix_refresh_tokens_token");
            entity.HasIndex(x => x.JwtId).HasDatabaseName("ix_refresh_tokens_jwt_id");
            entity.HasIndex(x => x.UserId).HasDatabaseName("ix_refresh_tokens_user_id");
            entity.HasIndex(x => x.ExpiresAt).HasDatabaseName("ix_refresh_tokens_expires_at");
            entity.HasIndex(x => x.IsUsed).HasDatabaseName("ix_refresh_tokens_is_used");
            entity.HasIndex(x => x.IsRevoked).HasDatabaseName("ix_refresh_tokens_is_revoked");

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
            entity.ToTable("email_verifications", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.Token)
                .HasColumnName("token")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(x => x.TokenHash)
                .HasColumnName("token_hash")
                .IsRequired()
                .HasMaxLength(255);

            // Optional fields
            entity.Property(x => x.VerifiedByIp).HasColumnName("verified_by_ip").HasMaxLength(45);

            // Enum conversion
            entity.Property(x => x.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            entity.HasIndex(x => x.TokenHash).HasDatabaseName("ix_email_verifications_token_hash");
            entity.HasIndex(x => x.UserId).HasDatabaseName("ix_email_verifications_user_id");
            entity.HasIndex(x => x.Email).HasDatabaseName("ix_email_verifications_email");
            entity.HasIndex(x => x.ExpiresAt).HasDatabaseName("ix_email_verifications_expires_at");
            entity.HasIndex(x => x.IsVerified).HasDatabaseName("ix_email_verifications_is_verified");

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
            entity.ToTable("roles", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Required fields
            entity.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(500);

            // Indexes
            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("ix_roles_name");

            entity.ConfigureAuditFields();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles", "user");
            
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            // Composite unique constraint
            entity.HasIndex(x => new { x.UserId, x.RoleId })
                .IsUnique()
                .HasDatabaseName("ix_user_roles_user_role");

            // Performance indexes
            entity.HasIndex(x => x.UserId).HasDatabaseName("ix_user_roles_user_id");
            entity.HasIndex(x => x.RoleId).HasDatabaseName("ix_user_roles_role_id");

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
        entity.Property("CreatedAt").HasColumnName("created_at").IsRequired();
        entity.Property("CreatedBy").HasColumnName("created_by").HasMaxLength(255);
        entity.Property("UpdatedAt").HasColumnName("updated_at");
        entity.Property("UpdatedBy").HasColumnName("updated_by").HasMaxLength(255);
    }

    private static void ConfigureSoftDelete<T>(this Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> entity) 
        where T : class
    {
        entity.Property("IsDeleted").HasColumnName("is_deleted").HasDefaultValue(false);
        entity.Property("DeletedAt").HasColumnName("deleted_at");
        entity.Property("DeletedBy").HasColumnName("deleted_by").HasMaxLength(255);
        
        // Global query filter for soft delete
        // TODO: Implement global query filter in DbContext
        // entity.HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
    }
}
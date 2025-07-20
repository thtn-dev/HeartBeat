using ZSocialMedia.Infrastructure.Database.Functions;
using ZSocialMedia.Infrastructure.Database.Extensions;
using ZSocialMedia.Shared;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Domain.PostModule.Entities;

namespace ZSocialMedia.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // User Module
    public DbSet<User> Users { get; set; }
    public DbSet<UserCredential> UserCredentials { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<UserFollower> UserFollowers { get; set; }
    public DbSet<UserBlock> UserBlocks { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<Role> Roles { get; set; }

    // Post Module
    // public DbSet<Post> Posts { get; set; }
    // public DbSet<Comment> Comments { get; set; }
    // public DbSet<PostLike> PostLikes { get; set; }
    // public DbSet<PostBookmark> PostBookmarks { get; set; }
    // public DbSet<PostRepost> PostReposts { get; set; }
    // public DbSet<PostView> PostViews { get; set; }
    // public DbSet<PostMedia> PostMedia { get; set; }
    // public DbSet<PostHashtag> PostHashtags { get; set; }
    // public DbSet<PostMention> PostMentions { get; set; }
    // public DbSet<Hashtag> Hashtags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set default schema
        modelBuilder.HasDefaultSchema("public");

        // Configure PostgreSQL-specific features
        ConfigurePostgreSqlFeatures(modelBuilder);

        // Configure entities using Fluent API extensions
        modelBuilder.ConfigureEntities();

        // Configure audit fields for all entities
        ConfigureAuditFields(modelBuilder);

        // Apply any additional configurations from assembly

        base.OnModelCreating(modelBuilder);
    }
    /// <summary>
    /// Configure PostgreSQL-specific features and optimizations
    /// </summary>
    private static void ConfigurePostgreSqlFeatures(ModelBuilder modelBuilder)
    {
        // Enable case-insensitive text operations using our custom function

        var normalizeTextMethod = typeof(DatabaseFunctions).GetMethod(nameof(DatabaseFunctions.NormalizeText), BindingFlags.Public | BindingFlags.Static);
        if (normalizeTextMethod is not null)
        {
            modelBuilder.HasDbFunction(normalizeTextMethod)
                .HasName("normalize_text");
        }

        // Configure sequences for sequential IDs
        // These sequences will be used for user IDs, post IDs, etc.
        modelBuilder.HasSequence<long>("user_id_seq")
            .StartsAt(1_000_000) // Start from 1 million to have consistent length
            .IncrementsBy(1)
            .HasMin(1_000_000)
            .HasMax(long.MaxValue);

        modelBuilder.HasSequence<long>("post_id_seq")
            .StartsAt(1_000_000)
            .IncrementsBy(1)
            .HasMin(1_000_000)
            .HasMax(long.MaxValue);

        // Enable full-text search configurations
        // This sets up text search for content discovery
        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.HasPostgresExtension("unaccent");
    }

    /// <summary>
    /// Configure audit fields (CreatedAt, UpdatedAt) for all entities
    /// </summary>
    private static void ConfigureAuditFields(ModelBuilder modelBuilder)
    {
        // This will automatically add audit trail to all entities
        // that implement IAuditableEntity interface
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType)) continue;
            // Configure CreatedAt to be set automatically on insert
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(IAuditableEntity.CreatedAt))
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            // Configure UpdatedAt to be set automatically on update
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(IAuditableEntity.UpdatedAt))
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        }
    }

    /// <summary>
    /// Override SaveChanges to implement custom audit logic
    /// This ensures data consistency and proper audit trail
    /// </summary>
    public override int SaveChanges()
    {
        ProcessAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Async version of SaveChanges with audit processing
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcessAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Process audit fields before saving changes
    /// This method updates CreatedAt and UpdatedAt timestamps appropriately
    /// </summary>
    private void ProcessAuditFields()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Set both CreatedAt and UpdatedAt for new entities
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    break;

                case EntityState.Modified:
                    // Only update UpdatedAt for modified entities
                    // CreatedAt should never change after entity creation
                    entry.Entity.UpdatedAt = utcNow;
                    // Prevent modification of CreatedAt
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

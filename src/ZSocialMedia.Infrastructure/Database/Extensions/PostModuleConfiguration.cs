using Microsoft.EntityFrameworkCore;
using ZSocialMedia.Domain.PostModule.Entities;

namespace ZSocialMedia.Infrastructure.Database.Extensions;

public static class PostModuleConfiguration
{
    internal static void ConfigurePost(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("posts");

            // Primary key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            // Content properties
            entity.Property(e => e.Content)
                .HasColumnName("content")
                .HasMaxLength(280)
                .IsRequired();

            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Format)
                .HasColumnName("format")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Foreign keys
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.ParentPostId)
                .HasColumnName("parent_post_id");

            entity.Property(e => e.ThreadRootId)
                .HasColumnName("thread_root_id");

            entity.Property(e => e.QuotedPostId)
                .HasColumnName("quoted_post_id");

            // Thread properties
            entity.Property(e => e.ReplyDepth)
                .HasColumnName("reply_depth")
                .HasDefaultValue(0);

            // Visibility & Privacy
            entity.Property(e => e.Visibility)
                .HasColumnName("visibility")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.IsCommentsEnabled)
                .HasColumnName("is_comments_enabled")
                .HasDefaultValue(true);

            entity.Property(e => e.CommentPermission)
                .HasColumnName("comment_permission")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Status
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.PublishedAt)
                .HasColumnName("published_at");

            entity.Property(e => e.ScheduledAt)
                .HasColumnName("scheduled_at");

            entity.Property(e => e.IsPinned)
                .HasColumnName("is_pinned")
                .HasDefaultValue(false);

            entity.Property(e => e.PinnedAt)
                .HasColumnName("pinned_at");

            // Metrics - using int for counts up to 2.1 billion
            entity.Property(e => e.LikesCount)
                .HasColumnName("likes_count")
                .HasDefaultValue(0);

            entity.Property(e => e.CommentsCount)
                .HasColumnName("comments_count")
                .HasDefaultValue(0);

            entity.Property(e => e.RepostsCount)
                .HasColumnName("reposts_count")
                .HasDefaultValue(0);

            entity.Property(e => e.QuotesCount)
                .HasColumnName("quotes_count")
                .HasDefaultValue(0);

            entity.Property(e => e.ViewsCount)
                .HasColumnName("views_count")
                .HasDefaultValue(0);

            entity.Property(e => e.SharesCount)
                .HasColumnName("shares_count")
                .HasDefaultValue(0);

            entity.Property(e => e.BookmarksCount)
                .HasColumnName("bookmarks_count")
                .HasDefaultValue(0);

            // Content flags
            entity.Property(e => e.HasMedia)
                .HasColumnName("has_media")
                .HasDefaultValue(false);

            entity.Property(e => e.HasLinks)
                .HasColumnName("has_links")
                .HasDefaultValue(false);

            entity.Property(e => e.HasMentions)
                .HasColumnName("has_mentions")
                .HasDefaultValue(false);

            entity.Property(e => e.HasHashtags)
                .HasColumnName("has_hashtags")
                .HasDefaultValue(false);

            entity.Property(e => e.HasPoll)
                .HasColumnName("has_poll")
                .HasDefaultValue(false);

            // Moderation
            entity.Property(e => e.IsReported)
                .HasColumnName("is_reported")
                .HasDefaultValue(false);

            entity.Property(e => e.ReportCount)
                .HasColumnName("report_count")
                .HasDefaultValue(0);

            entity.Property(e => e.IsSensitive)
                .HasColumnName("is_sensitive")
                .HasDefaultValue(false);

            entity.Property(e => e.IsNSFW)
                .HasColumnName("is_nsfw")
                .HasDefaultValue(false);

            entity.Property(e => e.ContentRating)
                .HasColumnName("content_rating")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // SEO & Discovery
            entity.Property(e => e.Slug)
                .HasColumnName("slug")
                .HasMaxLength(300);

            entity.Property(e => e.PreviewText)
                .HasColumnName("preview_text")
                .HasMaxLength(100);

            // Audit fields
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .HasMaxLength(100);

            // Soft delete
            entity.Property(e => e.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at");

            entity.Property(e => e.DeletedBy)
                .HasColumnName("deleted_by")
                .HasMaxLength(100);

            // Relationships
            entity.HasOne(e => e.Author)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ParentPost)
                .WithMany()
                .HasForeignKey(e => e.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.QuotedPost)
                .WithMany()
                .HasForeignKey(e => e.QuotedPostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.UserId).HasDatabaseName("ix_posts_user_id");
            entity.HasIndex(e => e.PublishedAt).HasDatabaseName("ix_posts_published_at");
            entity.HasIndex(e => new { e.UserId, e.Status, e.PublishedAt })
                .HasDatabaseName("ix_posts_user_status_published");
            entity.HasIndex(e => e.ThreadRootId).HasDatabaseName("ix_posts_thread_root_id");
            entity.HasIndex(e => e.Slug)
                .IsUnique()
                .HasDatabaseName("ix_posts_slug")
                .HasFilter("slug IS NOT NULL");
            entity.HasIndex(e => new { e.Status, e.PublishedAt })
                .HasDatabaseName("ix_posts_status_published")
                .HasFilter("status = 'Published'");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_posts_created_at");
            
            // Partial indexes for performance
            entity.HasIndex(e => new { e.IsPinned, e.UserId })
                .HasDatabaseName("ix_posts_pinned")
                .HasFilter("is_pinned = true");
            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("ix_posts_deleted")
                .HasFilter("is_deleted = false");

            // Global query filter
            // entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    internal static void ConfigureComment(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            // Foreign keys
            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.ParentCommentId)
                .HasColumnName("parent_comment_id");

            // Content
            entity.Property(e => e.Content)
                .HasColumnName("content")
                .HasMaxLength(280)
                .IsRequired();

            entity.Property(e => e.Format)
                .HasColumnName("format")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Thread info
            entity.Property(e => e.Depth)
                .HasColumnName("depth")
                .HasDefaultValue(0);

            entity.Property(e => e.ThreadPath)
                .HasColumnName("thread_path")
                .HasMaxLength(500); // Format: /root_id/parent_id/comment_id

            // Metrics
            entity.Property(e => e.LikesCount)
                .HasColumnName("likes_count")
                .HasDefaultValue(0);

            entity.Property(e => e.RepliesCount)
                .HasColumnName("replies_count")
                .HasDefaultValue(0);

            // Status
            entity.Property(e => e.IsEdited)
                .HasColumnName("is_edited")
                .HasDefaultValue(false);

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at");

            entity.Property(e => e.IsPinned)
                .HasColumnName("is_pinned")
                .HasDefaultValue(false);

            entity.Property(e => e.IsHighlighted)
                .HasColumnName("is_highlighted")
                .HasDefaultValue(false);

            // Moderation
            entity.Property(e => e.IsHidden)
                .HasColumnName("is_hidden")
                .HasDefaultValue(false);

            entity.Property(e => e.HiddenReason)
                .HasColumnName("hidden_reason")
                .HasMaxLength(200);

            entity.Property(e => e.IsReported)
                .HasColumnName("is_reported")
                .HasDefaultValue(false);

            entity.Property(e => e.ReportCount)
                .HasColumnName("report_count")
                .HasDefaultValue(0);

            // Audit fields
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .HasMaxLength(100);

            // Soft delete
            entity.Property(e => e.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at");

            entity.Property(e => e.DeletedBy)
                .HasColumnName("deleted_by")
                .HasMaxLength(100);

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Author)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => new { e.PostId, e.CreatedAt })
                .HasDatabaseName("ix_comments_post_created");
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("ix_comments_user_id");
            entity.HasIndex(e => e.ParentCommentId)
                .HasDatabaseName("ix_comments_parent_id");
            entity.HasIndex(e => e.ThreadPath)
                .HasDatabaseName("ix_comments_thread_path");
            entity.HasIndex(e => new { e.PostId, e.IsPinned })
                .HasDatabaseName("ix_comments_post_pinned")
                .HasFilter("is_pinned = true");

            // Global query filter
            // entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    internal static void ConfigureCommentLike(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommentLike>(entity =>
        {
            entity.ToTable("comment_likes");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CommentId)
                .HasColumnName("comment_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.LikedAt)
                .HasColumnName("liked_at")
                .IsRequired();

            // Relationships
            entity.HasOne(e => e.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // entity.HasOne(e => e.User)
            //     .WithMany()
            //     .HasForeignKey(e => e.UserId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.CommentId, e.UserId })
                .IsUnique()
                .HasDatabaseName("ix_comment_likes_comment_user");
            entity.HasIndex(e => new { e.UserId, e.LikedAt })
                .HasDatabaseName("ix_comment_likes_user_liked");
        });
    }

    internal static void ConfigurePostLike(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.ToTable("post_likes");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.LikedAt)
                .HasColumnName("liked_at")
                .IsRequired();

            entity.Property(e => e.Source)
                .HasColumnName("source")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.PostId, e.UserId })
                .IsUnique()
                .HasDatabaseName("ix_post_likes_post_user");
            entity.HasIndex(e => new { e.UserId, e.LikedAt })
                .HasDatabaseName("ix_post_likes_user_liked");
            entity.HasIndex(e => new { e.PostId, e.LikedAt })
                .HasDatabaseName("ix_post_likes_post_liked");
        });
    }

    internal static void ConfigurePostBookmark(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostBookmark>(entity =>
        {
            entity.ToTable("post_bookmarks");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            // Organization
            entity.Property(e => e.FolderName)
                .HasColumnName("folder_name")
                .HasMaxLength(100);

            entity.Property(e => e.Note)
                .HasColumnName("note")
                .HasMaxLength(500);

            entity.Property(e => e.Tags)
                .HasColumnName("tags")
                .HasColumnType("jsonb");

            entity.Property(e => e.BookmarkedAt)
                .HasColumnName("bookmarked_at")
                .IsRequired();

            // Audit fields
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .HasMaxLength(100);

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Bookmarks)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.UserId, e.PostId })
                .IsUnique()
                .HasDatabaseName("ix_post_bookmarks_user_post");
            entity.HasIndex(e => new { e.UserId, e.BookmarkedAt })
                .HasDatabaseName("ix_post_bookmarks_user_bookmarked");
            entity.HasIndex(e => new { e.UserId, e.FolderName })
                .HasDatabaseName("ix_post_bookmarks_user_folder");
        });
    }

    internal static void ConfigurePostRepost(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostRepost>(entity =>
        {
            entity.ToTable("post_reposts");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.RepostedAt)
                .HasColumnName("reposted_at")
                .IsRequired();

            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Comment)
                .HasColumnName("comment")
                .HasMaxLength(280);

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Reposts)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.PostId, e.UserId })
                .IsUnique()
                .HasDatabaseName("ix_post_reposts_post_user");
            entity.HasIndex(e => new { e.UserId, e.RepostedAt })
                .HasDatabaseName("ix_post_reposts_user_reposted");
        });
    }

    internal static void ConfigurePostView(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostView>(entity =>
        {
            entity.ToTable("post_views");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            // View details
            entity.Property(e => e.ViewedAt)
                .HasColumnName("viewed_at")
                .IsRequired();

            entity.Property(e => e.DurationSeconds)
                .HasColumnName("duration_seconds")
                .HasDefaultValue(0);

            entity.Property(e => e.Source)
                .HasColumnName("source")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.SessionId)
                .HasColumnName("session_id")
                .HasMaxLength(100);

            entity.Property(e => e.IpAddressHash)
                .HasColumnName("ip_address_hash")
                .HasMaxLength(64); // SHA256 hash

            entity.Property(e => e.Country)
                .HasColumnName("country")
                .HasMaxLength(2); // ISO country code

            // Device info
            entity.Property(e => e.DeviceType)
                .HasColumnName("device_type")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.UserAgent)
                .HasColumnName("user_agent")
                .HasMaxLength(500);

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(e => new { e.PostId, e.ViewedAt })
                .HasDatabaseName("ix_post_views_post_viewed");
            entity.HasIndex(e => new { e.UserId, e.ViewedAt })
                .HasDatabaseName("ix_post_views_user_viewed")
                .HasFilter("user_id IS NOT NULL");
            entity.HasIndex(e => e.ViewedAt)
                .HasDatabaseName("ix_post_views_viewed_at");
        });
    }

    internal static void ConfigurePostMedia(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostMedia>(entity =>
        {
            entity.ToTable("post_media");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            // Media info
            entity.Property(e => e.MediaUrl)
                .HasColumnName("media_url")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.ThumbnailUrl)
                .HasColumnName("thumbnail_url")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.MediaType)
                .HasColumnName("media_type")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.MimeType)
                .HasColumnName("mime_type")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.FileSize)
                .HasColumnName("file_size")
                .IsRequired();

            entity.Property(e => e.Width)
                .HasColumnName("width");

            entity.Property(e => e.Height)
                .HasColumnName("height");

            entity.Property(e => e.Duration)
                .HasColumnName("duration");

            // Processing
            entity.Property(e => e.ProcessingStatus)
                .HasColumnName("processing_status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ProcessingError)
                .HasColumnName("processing_error")
                .HasMaxLength(500);

            entity.Property(e => e.ProcessedAt)
                .HasColumnName("processed_at");

            // Metadata
            entity.Property(e => e.AltText)
                .HasColumnName("alt_text")
                .HasMaxLength(500);

            entity.Property(e => e.Caption)
                .HasColumnName("caption")
                .HasMaxLength(280);

            entity.Property(e => e.DisplayOrder)
                .HasColumnName("display_order")
                .HasDefaultValue(0);

            entity.Property(e => e.BlurHash)
                .HasColumnName("blur_hash")
                .HasMaxLength(100);

            // Moderation
            entity.Property(e => e.IsNSFW)
                .HasColumnName("is_nsfw")
                .HasDefaultValue(false);

            entity.Property(e => e.NSFWScore)
                .HasColumnName("nsfw_score")
                .HasPrecision(5, 4); // 0.0000 to 1.0000

            entity.Property(e => e.IsReviewed)
                .HasColumnName("is_reviewed")
                .HasDefaultValue(false);

            // Storage
            entity.Property(e => e.StorageKey)
                .HasColumnName("storage_key")
                .HasMaxLength(500);

            entity.Property(e => e.CDNUrl)
                .HasColumnName("cdn_url")
                .HasMaxLength(500);

            // Audit fields
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .HasMaxLength(100);

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Media)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.PostId, e.DisplayOrder })
                .HasDatabaseName("ix_post_media_post_order");
            entity.HasIndex(e => e.ProcessingStatus)
                .HasDatabaseName("ix_post_media_processing_status");
            entity.HasIndex(e => e.StorageKey)
                .HasDatabaseName("ix_post_media_storage_key");
        });
    }

    internal static void ConfigurePostHashtag(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostHashtag>(entity =>
        {
            entity.ToTable("post_hashtags");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            entity.Property(e => e.HashtagId)
                .HasColumnName("hashtag_id")
                .IsRequired();

            // Position in text
            entity.Property(e => e.StartPosition)
                .HasColumnName("start_position")
                .IsRequired();

            entity.Property(e => e.Length)
                .HasColumnName("length")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Hashtags)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Hashtag)
                .WithMany(h => h.PostHashtags)
                .HasForeignKey(e => e.HashtagId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => new { e.PostId, e.HashtagId })
                .HasDatabaseName("ix_post_hashtags_post_hashtag");
            entity.HasIndex(e => new { e.HashtagId, e.CreatedAt })
                .HasDatabaseName("ix_post_hashtags_hashtag_created");
        });
    }

    internal static void ConfigurePostMention(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostMention>(entity =>
        {
            entity.ToTable("post_mentions");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            entity.Property(e => e.MentionedUserId)
                .HasColumnName("mentioned_user_id")
                .IsRequired();

            // Mention details
            entity.Property(e => e.StartPosition)
                .HasColumnName("start_position")
                .IsRequired();

            entity.Property(e => e.Length)
                .HasColumnName("length")
                .IsRequired();

            entity.Property(e => e.IsNotified)
                .HasColumnName("is_notified")
                .HasDefaultValue(false);

            entity.Property(e => e.NotifiedAt)
                .HasColumnName("notified_at");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            // Relationships
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Mentions)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.MentionedUser)
                .WithMany()
                .HasForeignKey(e => e.MentionedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.PostId, e.MentionedUserId })
                .HasDatabaseName("ix_post_mentions_post_user");
            entity.HasIndex(e => new { e.MentionedUserId, e.CreatedAt })
                .HasDatabaseName("ix_post_mentions_user_created");
            entity.HasIndex(e => new { e.MentionedUserId, e.IsNotified })
                .HasDatabaseName("ix_post_mentions_user_notified")
                .HasFilter("is_notified = false");
        });
    }

    internal static void ConfigureHashtag(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.ToTable("hashtags");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Tag)
                .HasColumnName("tag")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.DisplayTag)
                .HasColumnName("display_tag")
                .HasMaxLength(100)
                .IsRequired();

            // Metrics - using bigint for very popular hashtags
            entity.Property(e => e.TotalUsageCount)
                .HasColumnName("total_usage_count")
                .HasDefaultValue(0L);

            entity.Property(e => e.DailyUsageCount)
                .HasColumnName("daily_usage_count")
                .HasDefaultValue(0L);

            entity.Property(e => e.WeeklyUsageCount)
                .HasColumnName("weekly_usage_count")
                .HasDefaultValue(0L);

            entity.Property(e => e.LastUsedAt)
                .HasColumnName("last_used_at");

            entity.Property(e => e.TrendingStartedAt)
                .HasColumnName("trending_started_at");

            entity.Property(e => e.IsTrending)
                .HasColumnName("is_trending")
                .HasDefaultValue(false);

            entity.Property(e => e.TrendingRank)
                .HasColumnName("trending_rank")
                .HasDefaultValue(0);

            // Categorization
            entity.Property(e => e.Category)
                .HasColumnName("category")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            entity.Property(e => e.RelatedTags)
                .HasColumnName("related_tags")
                .HasColumnType("jsonb");

            // Moderation
            entity.Property(e => e.IsBanned)
                .HasColumnName("is_banned")
                .HasDefaultValue(false);

            entity.Property(e => e.BannedReason)
                .HasColumnName("banned_reason")
                .HasMaxLength(200);

            entity.Property(e => e.IsPromoted)
                .HasColumnName("is_promoted")
                .HasDefaultValue(false);

            entity.Property(e => e.PromotedUntil)
                .HasColumnName("promoted_until");

            // Audit fields
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName("created_by")
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.UpdatedBy)
                .HasColumnName("updated_by")
                .HasMaxLength(100);

            // Indexes
            entity.HasIndex(e => e.Tag)
                .IsUnique()
                .HasDatabaseName("ix_hashtags_tag");
            entity.HasIndex(e => e.TrendingRank)
                .HasDatabaseName("ix_hashtags_trending_rank")
                .HasFilter("is_trending = true");
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("ix_hashtags_category");
            entity.HasIndex(e => e.TotalUsageCount)
                .HasDatabaseName("ix_hashtags_total_usage")
                .IsDescending();
            entity.HasIndex(e => new { e.IsBanned, e.Category })
                .HasDatabaseName("ix_hashtags_banned_category")
                .HasFilter("is_banned = false");
        });
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZSocialMedia.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.EnsureSchema(
                name: "user");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.CreateSequence(
                name: "post_id_seq",
                schema: "public",
                startValue: 1000000L,
                minValue: 1000000L,
                maxValue: 9223372036854775807L);

            migrationBuilder.CreateSequence(
                name: "user_id_seq",
                schema: "public",
                startValue: 1000000L,
                minValue: 1000000L,
                maxValue: 9223372036854775807L);

            migrationBuilder.CreateTable(
                name: "hashtags",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    total_usage_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    daily_usage_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    weekly_usage_count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    trending_started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_trending = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    trending_rank = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    related_tags = table.Column<string>(type: "jsonb", nullable: true),
                    is_banned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    banned_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_promoted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    promoted_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hashtags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    EmailVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    SuspendedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuspensionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsTwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorSecretKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LockoutEndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerifications",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerifiedByIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    LastAttemptAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    format = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    parent_post_id = table.Column<Guid>(type: "uuid", nullable: true),
                    thread_root_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reply_depth = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    quoted_post_id = table.Column<Guid>(type: "uuid", nullable: true),
                    visibility = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_comments_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    comment_permission = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    pinned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    likes_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    comments_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    reposts_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    quotes_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    views_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    shares_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    bookmarks_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    has_media = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    has_links = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    has_mentions = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    has_hashtags = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    has_poll = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_reported = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    report_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_sensitive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_nsfw = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    content_rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    slug = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    preview_text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_posts_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_posts_posts_parent_post_id",
                        column: x => x.parent_post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_posts_posts_quoted_post_id",
                        column: x => x.quoted_post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserBlocks",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockedId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReasonDetails = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlocks", x => x.Id);
                    table.CheckConstraint("CK_UserBlocks_NoSelfBlock", "\"BlockerId\" != \"BlockedId\"");
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_BlockedId",
                        column: x => x.BlockedId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_BlockerId",
                        column: x => x.BlockerId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredentials",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordSalt = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PasswordExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastFailedLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockoutEndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequirePasswordChange = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    LastLoginUserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    LastLoginLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredentials_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFollowers",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsNotificationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsCloseFriend = table.Column<bool>(type: "boolean", nullable: false),
                    IsPending = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowers", x => x.Id);
                    table.CheckConstraint("CK_UserFollowers_NoSelfFollow", "\"FollowerId\" != \"FollowingId\"");
                    table.ForeignKey(
                        name: "FK_UserFollowers_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollowers_Users_FollowingId",
                        column: x => x.FollowingId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    FollowersCount = table.Column<int>(type: "integer", nullable: false),
                    FollowingCount = table.Column<int>(type: "integer", nullable: false),
                    PostsCount = table.Column<int>(type: "integer", nullable: false),
                    LikesCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "user",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionToken = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DeviceModel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    LoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EndReason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsTrustedDevice = table.Column<bool>(type: "boolean", nullable: false),
                    FcmToken = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrivateAccount = table.Column<bool>(type: "boolean", nullable: false),
                    AllowDirectMessages = table.Column<bool>(type: "boolean", nullable: false),
                    DirectMessageFilter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ShowOnlineStatus = table.Column<bool>(type: "boolean", nullable: false),
                    ShowReadReceipts = table.Column<bool>(type: "boolean", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShowSensitiveContent = table.Column<bool>(type: "boolean", nullable: false),
                    QualityFilter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmailNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    PushNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultFeedSort = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ShowReposts = table.Column<bool>(type: "boolean", nullable: false),
                    ShowReplies = table.Column<bool>(type: "boolean", nullable: false),
                    AutoplayVideos = table.Column<bool>(type: "boolean", nullable: false),
                    AllowAnalytics = table.Column<bool>(type: "boolean", nullable: false),
                    AllowPersonalization = table.Column<bool>(type: "boolean", nullable: false),
                    DataDownloadFrequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_comment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    content = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: false),
                    format = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    depth = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    thread_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    likes_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    replies_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_highlighted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_hidden = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    hidden_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_reported = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    report_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_comments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalSchema: "public",
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_bookmarks",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    folder_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tags = table.Column<string>(type: "jsonb", nullable: true),
                    bookmarked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_bookmarks", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_bookmarks_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_bookmarks_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_hashtags",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hashtag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_position = table.Column<int>(type: "integer", nullable: false),
                    length = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_hashtags", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_hashtags_hashtags_hashtag_id",
                        column: x => x.hashtag_id,
                        principalSchema: "public",
                        principalTable: "hashtags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_post_hashtags_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_likes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    liked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_likes_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_likes_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_media",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    thumbnail_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: true),
                    height = table.Column<int>(type: "integer", nullable: true),
                    duration = table.Column<int>(type: "integer", nullable: true),
                    processing_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    processing_error = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alt_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    caption = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    blur_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_nsfw = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    nsfw_score = table.Column<float>(type: "real", precision: 5, scale: 4, nullable: true),
                    is_reviewed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cdn_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_media", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_media_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_mentions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mentioned_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_position = table.Column<int>(type: "integer", nullable: false),
                    length = table.Column<int>(type: "integer", nullable: false),
                    is_notified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    notified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_mentions", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_mentions_Users_mentioned_user_id",
                        column: x => x.mentioned_user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_mentions_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_reposts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reposted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    comment = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_reposts", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_reposts_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_reposts_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_views",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    viewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    session_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ip_address_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    device_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_views", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_views_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_post_views_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "public",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    JwtId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UsedByIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PreviousToken = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_UserSessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "user",
                        principalTable: "UserSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment_likes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    comment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    liked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_likes_Users_user_id",
                        column: x => x.user_id,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comment_likes_comments_comment_id",
                        column: x => x.comment_id,
                        principalSchema: "public",
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_comment_likes_comment_user",
                schema: "public",
                table: "comment_likes",
                columns: new[] { "comment_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_comment_likes_user_liked",
                schema: "public",
                table: "comment_likes",
                columns: new[] { "user_id", "liked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_comments_parent_id",
                schema: "public",
                table: "comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_post_created",
                schema: "public",
                table: "comments",
                columns: new[] { "post_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_comments_post_pinned",
                schema: "public",
                table: "comments",
                columns: new[] { "post_id", "is_pinned" },
                filter: "is_pinned = true");

            migrationBuilder.CreateIndex(
                name: "ix_comments_thread_path",
                schema: "public",
                table: "comments",
                column: "thread_path");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_id",
                schema: "public",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_Email",
                schema: "user",
                table: "EmailVerifications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_ExpiresAt",
                schema: "user",
                table: "EmailVerifications",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_IsVerified",
                schema: "user",
                table: "EmailVerifications",
                column: "IsVerified");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_TokenHash",
                schema: "user",
                table: "EmailVerifications",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_UserId",
                schema: "user",
                table: "EmailVerifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_hashtags_banned_category",
                schema: "public",
                table: "hashtags",
                columns: new[] { "is_banned", "category" },
                filter: "is_banned = false");

            migrationBuilder.CreateIndex(
                name: "ix_hashtags_category",
                schema: "public",
                table: "hashtags",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_hashtags_tag",
                schema: "public",
                table: "hashtags",
                column: "tag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_hashtags_total_usage",
                schema: "public",
                table: "hashtags",
                column: "total_usage_count",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_hashtags_trending_rank",
                schema: "public",
                table: "hashtags",
                column: "trending_rank",
                filter: "is_trending = true");

            migrationBuilder.CreateIndex(
                name: "IX_post_bookmarks_post_id",
                schema: "public",
                table: "post_bookmarks",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_bookmarks_user_bookmarked",
                schema: "public",
                table: "post_bookmarks",
                columns: new[] { "user_id", "bookmarked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_post_bookmarks_user_folder",
                schema: "public",
                table: "post_bookmarks",
                columns: new[] { "user_id", "folder_name" });

            migrationBuilder.CreateIndex(
                name: "ix_post_bookmarks_user_post",
                schema: "public",
                table: "post_bookmarks",
                columns: new[] { "user_id", "post_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_hashtags_hashtag_created",
                schema: "public",
                table: "post_hashtags",
                columns: new[] { "hashtag_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_post_hashtags_post_hashtag",
                schema: "public",
                table: "post_hashtags",
                columns: new[] { "post_id", "hashtag_id" });

            migrationBuilder.CreateIndex(
                name: "ix_post_likes_post_liked",
                schema: "public",
                table: "post_likes",
                columns: new[] { "post_id", "liked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_post_likes_post_user",
                schema: "public",
                table: "post_likes",
                columns: new[] { "post_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_likes_user_liked",
                schema: "public",
                table: "post_likes",
                columns: new[] { "user_id", "liked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_post_media_post_order",
                schema: "public",
                table: "post_media",
                columns: new[] { "post_id", "display_order" });

            migrationBuilder.CreateIndex(
                name: "ix_post_media_processing_status",
                schema: "public",
                table: "post_media",
                column: "processing_status");

            migrationBuilder.CreateIndex(
                name: "ix_post_media_storage_key",
                schema: "public",
                table: "post_media",
                column: "storage_key");

            migrationBuilder.CreateIndex(
                name: "ix_post_mentions_post_user",
                schema: "public",
                table: "post_mentions",
                columns: new[] { "post_id", "mentioned_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_post_mentions_user_created",
                schema: "public",
                table: "post_mentions",
                columns: new[] { "mentioned_user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_post_mentions_user_notified",
                schema: "public",
                table: "post_mentions",
                columns: new[] { "mentioned_user_id", "is_notified" },
                filter: "is_notified = false");

            migrationBuilder.CreateIndex(
                name: "ix_post_reposts_post_user",
                schema: "public",
                table: "post_reposts",
                columns: new[] { "post_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_reposts_user_reposted",
                schema: "public",
                table: "post_reposts",
                columns: new[] { "user_id", "reposted_at" });

            migrationBuilder.CreateIndex(
                name: "ix_post_views_post_viewed",
                schema: "public",
                table: "post_views",
                columns: new[] { "post_id", "viewed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_post_views_user_viewed",
                schema: "public",
                table: "post_views",
                columns: new[] { "user_id", "viewed_at" },
                filter: "user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_post_views_viewed_at",
                schema: "public",
                table: "post_views",
                column: "viewed_at");

            migrationBuilder.CreateIndex(
                name: "ix_posts_created_at",
                schema: "public",
                table: "posts",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_posts_deleted",
                schema: "public",
                table: "posts",
                column: "is_deleted",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "IX_posts_parent_post_id",
                schema: "public",
                table: "posts",
                column: "parent_post_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_pinned",
                schema: "public",
                table: "posts",
                columns: new[] { "is_pinned", "user_id" },
                filter: "is_pinned = true");

            migrationBuilder.CreateIndex(
                name: "ix_posts_published_at",
                schema: "public",
                table: "posts",
                column: "published_at");

            migrationBuilder.CreateIndex(
                name: "IX_posts_quoted_post_id",
                schema: "public",
                table: "posts",
                column: "quoted_post_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_slug",
                schema: "public",
                table: "posts",
                column: "slug",
                unique: true,
                filter: "slug IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_posts_status_published",
                schema: "public",
                table: "posts",
                columns: new[] { "status", "published_at" },
                filter: "status = 'Published'");

            migrationBuilder.CreateIndex(
                name: "ix_posts_thread_root_id",
                schema: "public",
                table: "posts",
                column: "thread_root_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_user_id",
                schema: "public",
                table: "posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_user_status_published",
                schema: "public",
                table: "posts",
                columns: new[] { "user_id", "status", "published_at" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                schema: "user",
                table: "RefreshTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_IsRevoked",
                schema: "user",
                table: "RefreshTokens",
                column: "IsRevoked");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_IsUsed",
                schema: "user",
                table: "RefreshTokens",
                column: "IsUsed");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_JwtId",
                schema: "user",
                table: "RefreshTokens",
                column: "JwtId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_SessionId",
                schema: "user",
                table: "RefreshTokens",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "user",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "user",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "user",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockedAt",
                schema: "user",
                table: "UserBlocks",
                column: "BlockedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockedId",
                schema: "user",
                table: "UserBlocks",
                column: "BlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockerBlocked",
                schema: "user",
                table: "UserBlocks",
                columns: new[] { "BlockerId", "BlockedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockerId",
                schema: "user",
                table: "UserBlocks",
                column: "BlockerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_LockoutEndAt",
                schema: "user",
                table: "UserCredentials",
                column: "LockoutEndAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_UserId",
                schema: "user",
                table: "UserCredentials",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_FollowedAt",
                schema: "user",
                table: "UserFollowers",
                column: "FollowedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_FollowerFollowing",
                schema: "user",
                table: "UserFollowers",
                columns: new[] { "FollowerId", "FollowingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_FollowerId",
                schema: "user",
                table: "UserFollowers",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_FollowingId",
                schema: "user",
                table: "UserFollowers",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowers_IsPending",
                schema: "user",
                table: "UserFollowers",
                column: "IsPending");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_DisplayName",
                schema: "user",
                table: "UserProfiles",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                schema: "user",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "user",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "user",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserRole",
                schema: "user",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                schema: "user",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "user",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                schema: "user",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                schema: "user",
                table: "Users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "user",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ExpiresAt",
                schema: "user",
                table: "UserSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_IsActive",
                schema: "user",
                table: "UserSessions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_LastActivityAt",
                schema: "user",
                table: "UserSessions",
                column: "LastActivityAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_SessionToken",
                schema: "user",
                table: "UserSessions",
                column: "SessionToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                schema: "user",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                schema: "user",
                table: "UserSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment_likes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EmailVerifications",
                schema: "user");

            migrationBuilder.DropTable(
                name: "post_bookmarks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "post_hashtags",
                schema: "public");

            migrationBuilder.DropTable(
                name: "post_likes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "post_media",
                schema: "public");

            migrationBuilder.DropTable(
                name: "post_mentions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "post_reposts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "post_views",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "user");

            migrationBuilder.DropTable(
                name: "UserBlocks",
                schema: "user");

            migrationBuilder.DropTable(
                name: "UserCredentials",
                schema: "user");

            migrationBuilder.DropTable(
                name: "UserFollowers",
                schema: "user");

            migrationBuilder.DropTable(
                name: "UserProfiles",
                schema: "user");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "user");

            migrationBuilder.DropTable(
                name: "UserSettings",
                schema: "user");

            migrationBuilder.DropTable(
                name: "comments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "hashtags",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserSessions",
                schema: "user");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "user");

            migrationBuilder.DropTable(
                name: "posts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "user");

            migrationBuilder.DropSequence(
                name: "post_id_seq",
                schema: "public");

            migrationBuilder.DropSequence(
                name: "user_id_seq",
                schema: "public");
        }
    }
}

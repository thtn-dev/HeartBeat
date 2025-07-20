using Microsoft.EntityFrameworkCore;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Domain.PostModule.Entities;

namespace ZSocialMedia.Infrastructure.Database.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigureEntities(this ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureUserModule();
       modelBuilder.ConfigurePostModule();
        
        return modelBuilder;
    }

    private static void ConfigureUserModule(this ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureUser();
        modelBuilder.ConfigureUserCredential();
        modelBuilder.ConfigureUserProfile();
        modelBuilder.ConfigureUserSettings();
        modelBuilder.ConfigureUserFollower();
        modelBuilder.ConfigureUserBlock();
        modelBuilder.ConfigureUserSession();
        modelBuilder.ConfigureRefreshToken();
        modelBuilder.ConfigureEmailVerification();
        modelBuilder.ConfigureRole();
    }

    private static void ConfigurePostModule(this ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigurePost();
        modelBuilder.ConfigureComment();
        modelBuilder.ConfigureCommentLike();
        modelBuilder.ConfigurePostLike();
        modelBuilder.ConfigurePostBookmark();
        modelBuilder.ConfigurePostRepost();
        modelBuilder.ConfigurePostView();
        modelBuilder.ConfigurePostMedia();
        modelBuilder.ConfigurePostHashtag();
        modelBuilder.ConfigurePostMention();
        modelBuilder.ConfigureHashtag();
    }
}
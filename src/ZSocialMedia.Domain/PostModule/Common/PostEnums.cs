namespace ZSocialMedia.Domain.PostModule.Common;

// Enums
public enum PostType
{
    Regular = 0,
    Reply = 1,
    Quote = 2,
    Thread = 3,
    Poll = 4,
    Announcement = 5
}

public enum ContentFormat
{
    PlainText = 0,
    Markdown = 1,
}

public enum PostVisibility
{
    Public = 0,
    Followers = 1,
    Following = 2,
    Mutual = 3,
    Private = 4,
    Mentioned = 5
}

public enum CommentPermission
{
    Everyone = 0,
    Following = 1,
    Followers = 2,
    Mentioned = 3,
    Nobody = 4
}

public enum PostStatus
{
    Draft = 0,
    Published = 1,
    Scheduled = 2,
    Hidden = 3,
    Removed = 4
}

public enum ContentRating
{
    General = 0,
    Sensitive = 1,
    Mature = 2,
    Restricted = 3
}

public enum MediaType
{
    Image = 0,
    Video = 1,
    Audio = 2,
    GIF = 3,
    Document = 4
}

public enum MediaProcessingStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public enum HashtagCategory
{
    General = 0,
    News = 1,
    Sports = 2,
    Entertainment = 3,
    Technology = 4,
    Politics = 5,
    Business = 6,
    Health = 7,
    Education = 8,
    Lifestyle = 9
}

public enum LikeSource
{
    Timeline = 0,
    PostDetail = 1,
    Search = 2,
    Profile = 3,
    Notification = 4
}

public enum RepostType
{
    Simple = 0,
    Quote = 1
}

public enum ViewSource
{
    Timeline = 0,
    PostDetail = 1,
    Profile = 2,
    Search = 3,
    Trending = 4,
    External = 5
}
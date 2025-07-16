namespace HeartBeat.Infrastructure.Caching.Constants;


public static class CacheKeys
{
    // User-related cache keys
    public const string USER_PROFILE = "user:profile:{0}";
    public const string USER_SESSION = "session:{0}";
    public const string USER_PREFERENCES = "user:prefs:{0}";
    public const string USER_STATS = "user:stats:{0}";

    // Social graph cache keys
    public const string USER_FOLLOWERS = "user:followers:{0}";
    public const string USER_FOLLOWING = "user:following:{0}";
    public const string USER_FOLLOWER_COUNT = "user:follower_count:{0}";
    public const string USER_FOLLOWING_COUNT = "user:following_count:{0}";

    // Feed cache keys
    public const string USER_FEED = "feed:user:{0}";
    public const string USER_FEED_METADATA = "feed:meta:{0}";
    public const string TRENDING_FEED = "feed:trending";
    public const string GLOBAL_FEED = "feed:global";

    // Post-related cache keys
    public const string POST_DETAILS = "post:{0}";
    public const string POST_LIKES = "post:likes:{0}";
    public const string POST_COMMENTS = "post:comments:{0}";
    public const string POST_SHARES = "post:shares:{0}";
    public const string POST_ENGAGEMENT = "post:engagement:{0}";

    // Content discovery cache keys
    public const string TRENDING_POSTS = "trending:posts";
    public const string TRENDING_HASHTAGS = "trending:hashtags";
    public const string POPULAR_USERS = "popular:users";
    public const string RECOMMENDED_USERS = "recommended:users:{0}";

    // Search cache keys
    public const string SEARCH_RESULTS = "search:{0}:{1}"; // query:page
    public const string SEARCH_SUGGESTIONS = "search:suggestions:{0}";

    // Media cache keys
    public const string MEDIA_METADATA = "media:{0}";
    public const string MEDIA_PROCESSING_STATUS = "media:status:{0}";

    // Analytics cache keys
    public const string USER_ANALYTICS = "analytics:user:{0}:{1}"; // userId:date
    public const string POST_ANALYTICS = "analytics:post:{0}:{1}"; // postId:date
    public const string GLOBAL_ANALYTICS = "analytics:global:{0}"; // date

    // Rate limiting cache keys
    public const string RATE_LIMIT_USER = "ratelimit:user:{0}:{1}"; // userId:action
    public const string RATE_LIMIT_IP = "ratelimit:ip:{0}:{1}"; // ip:action

    // Notification cache keys
    public const string USER_NOTIFICATIONS = "notifications:{0}";
    public const string NOTIFICATION_COUNT = "notifications:count:{0}";

    // Cache warming keys
    public const string WARM_QUEUE = "warming:queue";
    public const string WARM_STATUS = "warming:status:{0}";
}


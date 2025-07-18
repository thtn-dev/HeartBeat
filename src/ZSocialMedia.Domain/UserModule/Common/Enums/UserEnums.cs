namespace ZSocialMedia.Domain.UserModule.Common.Enums;

public enum DirectMessageFilter
{
    Everyone = 0,
    Following = 1,
    Verified = 2,
    Nobody = 3
}

public enum Language
{
    English = 0,
    Vietnamese = 1,
}

public enum ContentQualityFilter
{
    None = 0,
    Standard = 1,
    Strict = 2
}

public enum FeedSortOrder
{
    Chronological = 0,
    Algorithmic = 1,
    Popular = 2
}


public enum DataDownloadFrequency
{
    Never = 0,
    Monthly = 1,
    Quarterly = 2,
    Yearly = 3
}

public enum BlockReason
{
    Spam = 0,
    Harassment = 1,
    InappropriateContent = 2,
    FakeAccount = 3,
    Other = 4
}

public enum DeviceType
{
    Unknown = 0,
    Web = 1,
    MobileWeb = 2,
    AndroidApp = 3,
    iOSApp = 4,
    WindowsApp = 5,
    MacApp = 6,
    API = 7
}

public enum SessionEndReason
{
    UserLogout = 0,
    Expired = 1,
    AdminTerminated = 2,
    PasswordChanged = 3,
    SecurityConcern = 4,
    DeviceRemoved = 5
}

public enum VerificationType
{
    EmailConfirmation = 0,
    EmailChange = 1,
    PasswordReset = 2
}
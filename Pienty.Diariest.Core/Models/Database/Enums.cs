namespace Pienty.Diariest.Core.Models.Database
{
    public enum UserPermission : int
    {
        Client = 0,
        Agency = 1,
        Admin = 2
    }

    public enum Language : int
    {
        Turkish = 0,
        English = 1
    }

    public enum Platform : int
    {
        Meta = 0,
        LinkedIn = 1,
        Twitter = 2,
        TikTok = 3
    }

    public enum Currency : int
    {
        TRY = 0,
        USD = 1,
        EUR = 2,
        GBP = 3
    }

    #region API Response Message

    public enum APIMessage
    {
        WrongEmail,
        WrongPassword,
        UserDisabled,
        UserDeleted,
        MaxLoginLimit,
        SuccessLogin,
        Success,
        SuccessLogout,
        Error,
    }

    #endregion

    #region Meta Ads

    public enum AdStatus : int
    {
        Active = 0,
        Paused = 1,
        Deleted = 2,
        Archived = 3,
        AdsetPaused = 4,
        CampaignPaused = 5
    }

    public enum AdApiException : int
    {
        OAuthException = 0
    }

    #endregion
}
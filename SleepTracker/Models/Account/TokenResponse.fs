namespace SleeptTracker.Models.Account

type TokenResponse = {
    TokenType: string;
    AccessToken : string;
    ExpiresIn: int;
    RefreshToken: string;
}
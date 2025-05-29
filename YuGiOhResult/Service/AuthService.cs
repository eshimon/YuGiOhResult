using Microsoft.Identity.Client;
using YuGiOhResult;

public class AuthService
{
    // Azureで取得したクライアントID
    private const string ClientId = "987aff9b-e781-48ef-bd8c-18e501734358";
    // 必要なスコープ（例: OneDrive用）
    private readonly string[] Scopes = new[] { "Files.ReadWrite" };

    // MSALのPublicClientApplicationインスタンス
    private IPublicClientApplication _pca;

    public AuthService()
    {
        _pca = PublicClientApplicationBuilder
            .Create(ClientId)
            .WithRedirectUri("987aff9b-e781-48ef-bd8c-18e501734358://auth") // プラットフォームに合わせて変更
            .Build();
    }

    public async Task<string> SignInAndGetTokenAsync()
    {
        try
        {
            // 既存のアカウントでサイレント認証
            var accounts = await _pca.GetAccountsAsync();
            var result = await _pca.AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                                   .ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            // 初回やサイレント失敗時はインタラクティブ認証
            var result = await _pca.AcquireTokenInteractive(Scopes)
                                   .WithParentActivityOrWindow(App.Current.MainPage.Handler.PlatformView)
                                   .ExecuteAsync();
            return result.AccessToken;
        }
    }
}

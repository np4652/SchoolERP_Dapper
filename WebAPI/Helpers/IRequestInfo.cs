namespace WebAPI.Helpers
{
    public interface IRequestInfo
    {
        string GetRemoteIP();
        string GetLocalIP();
        string GetBrowser();
        string GetBrowserVersion();
        string GetUserAgent();
        string GetBrowserFullInfo();
        string GetDomain();
        string GetUserAgentMD5();
        string GetAbsoluteURI();
    }
}

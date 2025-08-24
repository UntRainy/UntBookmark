using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Steamworks;
using Unturned.SystemEx;
#if NETSTANDARD
using System.Net.Http;
#endif

namespace UntBookmark.Shared
{
    public class UntBookmark
    {
        public string BookmarkHost { get; private set; }
        public string LoginToken { get; private set; }

        public UntBookmark(string bookmarkHost, string loginToken)
        {
            if (string.IsNullOrEmpty(bookmarkHost))
                throw new ArgumentException("Bookmark host cannot be null or empty.", nameof(bookmarkHost));
            BookmarkHost = bookmarkHost;
            if (string.IsNullOrEmpty(loginToken))
                throw new ArgumentException("Login token cannot be null or empty.", nameof(loginToken));
            LoginToken = loginToken;
        }

        private string GetFakeIP()
        {
            SteamGameServerNetworkingSockets.GetFakeIP(0, out var ip);
            if (ip.m_eResult != EResult.k_EResultOK)
            {
                throw new ExternalException($"Failed to obtain server fake IP: {ip.m_eResult}");
            }
            var ipv4 = new IPv4Address(ip.m_unIP).ToString();
            return $"{ipv4}:{ip.m_unPorts[0]}";
        }

        public async Task<string> UpdateBookmarkIPAsync()
        {
            var ip = GetFakeIP();
#if NETSTANDARD
            var hc = new HttpClient()
            {
                DefaultRequestHeaders = {
                    { "X-GSLT", LoginToken },
                    { "User-Agent", "UntBookmark/1.0" }
                }
            };
            var res = await hc.PostAsync(BookmarkHost, new StringContent(ip));
            if (res.StatusCode != HttpStatusCode.NoContent)
                throw new WebException($"Failed to set bookmark IP: {res.StatusCode}");
#elif NETFRAMEWORK
            var wc = new WebClient();
            wc.Headers.Add("X-GSLT", LoginToken);
            wc.Headers.Add("User-Agent", "UntBookmark/1.0");
            try
            {
                await wc.UploadStringTaskAsync(BookmarkHost, ip);
            }
            catch (WebException e)
            {
                throw new WebException($"Failed to set bookmark IP: {e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred while setting the bookmark IP: {e.Message}");
            }
#endif
            return ip;
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using SDG.Unturned;

namespace UntBookmark.OpenMod
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class UntBookmarkService : IUntBookmarkService
    {
        private readonly ILogger<UntBookmarkService> m_Logger;

        private readonly Shared.UntBookmark? untBookmark;

        public UntBookmarkService(ILogger<UntBookmarkService> logger)
        {
            m_Logger = logger;

            if (!Provider.configData.Server.Use_FakeIP)
            {
                //m_Logger.LogInformation(m_StringLocalizer["messages:fake_ip_not_used"]);
                m_Logger.LogInformation("Bookmark host updating is disabled.");
                return;
            }

            if (string.IsNullOrEmpty(Provider.configData.Browser.Login_Token))
            {
                //m_Logger.LogError(m_StringLocalizer["messages:gslt_not_set"]);
                m_Logger.LogError("GSLT not set, bookmark host updating is disabled.");
                return;
            }
            if (string.IsNullOrEmpty(Provider.configData.Browser.BookmarkHost))
            {
                //m_Logger.LogError(m_StringLocalizer["messages:bookmark_host_not_set"]);
                m_Logger.LogError("Bookmark host not set, bookmark host updating is disabled.");
                return;
            }

            m_Logger.LogInformation("UntBookmark service initialized.");
            untBookmark = new UntBookmark.Shared.UntBookmark(Provider.configData.Browser.BookmarkHost, Provider.configData.Browser.Login_Token);
        }

        public async Task<string> UpdateBookmarkIPAsync()
        {
            if (untBookmark == null)
                throw new InvalidOperationException("UntBookmark is disabled.");
            return await untBookmark.UpdateBookmarkIPAsync();
        }
    }
}

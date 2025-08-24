using System;
using System.Threading.Tasks;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace UntBookmark.Rocket
{
    public class UntBookmarkRocketPlugin : RocketPlugin
    {
        public Shared.UntBookmark UntBookmark { private set; get; }

        public UntBookmarkRocketPlugin()
        {
            if (Provider.configData.Server.Use_FakeIP)
            {
                if (string.IsNullOrEmpty(Provider.configData.Browser.Login_Token))
                {
                    Logger.LogError("GSLT not set, bookmark host updating is disabled.");
                    return;
                }
                if (string.IsNullOrEmpty(Provider.configData.Browser.BookmarkHost))
                {
                    Logger.LogError("Bookmark host not set, bookmark host updating is disabled.");
                    return;
                }
                Logger.Log("UntBookmark service initialized.");
                UntBookmark = new Shared.UntBookmark(Provider.configData.Browser.BookmarkHost, Provider.configData.Browser.Login_Token);
            }
            else
            {
                Logger.Log("Bookmark host updating is disabled.");
            }
        }

        protected override void Load()
        {
            Level.onPostLevelLoaded += OnPostLevelLoaded;
        }

        protected override void Unload()
        {
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
        }

        private void OnPostLevelLoaded(int level)
        {
            if (UntBookmark == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    var ip = await UntBookmark.UpdateBookmarkIPAsync();
                    Logger.Log($"Successfully updated the bookmark IP to {ip}");
                }
                catch (Exception e)
                {
                    Logger.LogError($"An error occurred while updating the bookmark IP: {e.Message}");
                }
            });
        }
    }
}
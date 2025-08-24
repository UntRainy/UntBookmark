using System;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Level.Events;
using SDG.Unturned;
using OpenMod.API.Plugins;

[assembly: PluginMetadata("Rainy.Unturned.Bookmark", DisplayName = "Bookmark")]
namespace UntBookmark.OpenMod
{
    public class UntBookmarkOpenModPlugin : OpenModUnturnedPlugin
    {
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<UntBookmarkOpenModPlugin> m_Logger;
        private readonly IEventBus m_EventBus;
        private readonly IUntBookmarkService m_UntBookmarkService;

        public UntBookmarkOpenModPlugin(
            IStringLocalizer stringLocalizer,
            ILogger<UntBookmarkOpenModPlugin> logger,
            IEventBus eventBus,
            IUntBookmarkService untBookmarkService,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_EventBus = eventBus;
            m_UntBookmarkService = untBookmarkService;
        }

        protected override UniTask OnLoadAsync()
        {
            m_EventBus.Subscribe<UnturnedPostLevelLoadedEvent>(this, async (provider, sender, @event) =>
            {
                try
                {
                    var ip = await m_UntBookmarkService.UpdateBookmarkIPAsync();
                    m_Logger.LogInformation($"Bookmark IP has been set to {ip}");
                }
                catch (InvalidOperationException e)
                {
                    if (Provider.configData.Server.Use_FakeIP)
                        m_Logger.LogError(e, "Bookmark host not set or GSLT not set, bookmark host updating is disabled.");
                }
                catch (Exception e)
                {
                    m_Logger.LogError($"An error occurred while updating the bookmark IP: {e.Message}");
                }
            });
            return UniTask.CompletedTask;
        }
    }
}
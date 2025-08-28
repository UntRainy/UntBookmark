using System;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using SDG.Unturned;
using OpenMod.API.Plugins;
using Steamworks;
using System.Threading.Tasks;

[assembly: PluginMetadata("Rainy.Unturned.Bookmark", DisplayName = "Bookmark")]
namespace UntBookmark.OpenMod
{
    public class UntBookmarkOpenModPlugin : OpenModUnturnedPlugin
    {
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<UntBookmarkOpenModPlugin> m_Logger;
        private readonly IUntBookmarkService m_UntBookmarkService;

        public UntBookmarkOpenModPlugin(
            IStringLocalizer stringLocalizer,
            ILogger<UntBookmarkOpenModPlugin> logger,
            IUntBookmarkService untBookmarkService,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_UntBookmarkService = untBookmarkService;
        }

        protected override UniTask OnLoadAsync()
        {
            Shared.Hook.FakeIPCallback += OnFakeIPResult;
            Shared.Hook.Init();

            return UniTask.CompletedTask;
        }

        protected override UniTask OnUnloadAsync()
        {
            Shared.Hook.Uninit();
            Shared.Hook.FakeIPCallback -= OnFakeIPResult;
            return UniTask.CompletedTask;
        }

        private void OnFakeIPResult(SteamNetworkingFakeIPResult_t result)
        {
            if (result.m_eResult != EResult.k_EResultOK)
                return;
            Task.Run(async () =>
            {
                try
                {
                    var ip = await m_UntBookmarkService.UpdateBookmarkIPAsync(result);
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
        }
    }
}
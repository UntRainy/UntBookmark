using System;
using HarmonyLib;
using Steamworks;

namespace UntBookmark.Shared
{
    public static class Hook
    {
        private static readonly Harmony harmony = new Harmony("rainy.unt.bookmark");

        public static event Action<SteamNetworkingFakeIPResult_t> FakeIPCallback;

        private static Callback<SteamNetworkingFakeIPResult_t> fakeIPCallback;

        public static void Init()
        {
            harmony.PatchAll();
            OnAttachFakeIPCallback();

            try
            {
                SteamGameServerNetworkingSockets.GetFakeIP(0, out var info);
                if (info.m_eResult == EResult.k_EResultOK)
                    OnFakeIPResult(info);
            }
            catch { }
        }

        public static void Uninit()
        {
            harmony.UnpatchAll(harmony.Id);
        }

        private static void OnFakeIPResult(SteamNetworkingFakeIPResult_t result)
        {
            FakeIPCallback?.Invoke(result);
        }

        internal static void OnAttachFakeIPCallback()
        {
            OnDetachFakeIPCallback();
            fakeIPCallback = Callback<SteamNetworkingFakeIPResult_t>.CreateGameServer(OnFakeIPResult);
        }

        internal static void OnDetachFakeIPCallback()
        {
            fakeIPCallback?.Dispose();
            fakeIPCallback = null;
        }
    }

    [HarmonyPatch(typeof(GameServer), nameof(GameServer.Init))]
    public static class GameServer_Init_Patch
    {
        public static void Postfix()
        {
            Hook.OnAttachFakeIPCallback();
        }
    }

    [HarmonyPatch(typeof(GameServer), nameof(GameServer.Shutdown))]
    public static class GameServer_Shutdown_Patch
    {
        public static void Postfix()
        {
            Hook.OnDetachFakeIPCallback();
        }
    }
}
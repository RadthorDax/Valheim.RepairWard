using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace RepairWard
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class RepairWardPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "radthordax.valheim.RepairWard";
        public const string PluginName = "Repair Ward";
        public const string PluginVersion = "1.0.0";

        private static Harmony harmony;

        public static ConfigEntry<int> nexusID;
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<string> modKey;
        public static ConfigEntry<bool> repairPlayerBuiltOnly;
        //public static ConfigEntry<int> repairDuration;

        private void Awake()
        {
            nexusID = Config.Bind<int>("General", "NexusID", 322, "NexusMods ID for updates");
            modEnabled = Config.Bind<bool>("General", "Enabled", true, "Enable the mod");
            modKey = Config.Bind<string>("General", "ModifierKey", "left shift", "Modifier key to trigger the repair function of a ward. Find valid keycodes here: https://docs.unity3d.com/Manual/ConventionalGameInput.html");
            repairPlayerBuiltOnly = Config.Bind<bool>("General", "RepairPlayerBuiltOnly", false, "Prevent Wards repairing existing building structures placed by the game.");
            //repairDuration = Config.Bind<int>("General", "RepairDuration", 10, "Number of seconds to fully repair warded objects. Use 0 for instant repair.");

            if (!modEnabled.Value)
                return;

            harmony = new Harmony(PluginGUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        private static void Log(string msg)
        {
            Debug.Log($"[{PluginName}] {msg}");
        }

        [HarmonyPatch(typeof(PrivateArea), "GetHoverText")]
        static class PatchWardHoverText
        {
            static void Postfix(PrivateArea __instance, ref string __result)
            {
                if (__instance.IsEnabled())
                    __result += Localization.instance.Localize($"\n[{modKey.Value}+<color=yellow><b>$KEY_Use</b></color>] Repair Field");
            }
        }

        [HarmonyPatch(typeof(PrivateArea), "Interact")]
        static class PatchWardInteract
        {
            static bool Prefix(PrivateArea __instance)
            {
                if (KeyDown(modKey.Value) && __instance.IsEnabled())
                {
                    int repairs = 0;
                    List<Piece> nearbyPieces = new List<Piece>();
                    Piece.GetAllPiecesInRadius(__instance.transform.position, __instance.m_radius, nearbyPieces);
                    Log($"Found {nearbyPieces.Count} pieces in range of ward.");

                    foreach (Piece p in nearbyPieces)
                    {
                        try
                        {
                            if (!repairPlayerBuiltOnly.Value || p.IsPlacedByPlayer())
                            {
                                WearNTear wnt = p.GetComponentInParent<WearNTear>();
                                if (wnt?.GetHealthPercentage() < 1f)
                                {
                                    wnt.Repair();
                                    repairs++;
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    if (repairs > 0)
                    {
                        __instance.m_activateEffect.Create(__instance.transform.position, __instance.transform.rotation);
                        __instance.m_flashEffect.Create(__instance.transform.position, Quaternion.identity);
                        Log($"Repaired {repairs} warded pieces.");
                    }
                    else
                        Log("No warded pieces needed repair.");

                    return false;
                }

                return true;
            }
        }

        private static bool KeyDown(string key)
        {
            try { return Input.GetKey(key.ToLower()); }
            catch { return false; }
        }
    }
}

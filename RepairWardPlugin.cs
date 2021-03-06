using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace Valheim.RepairWard
{
    [BepInPlugin("com.radthordax.valheim.RepairWard", "Repair Ward", "1.0.0")]
    public class RepairWardPlugin : BaseUnityPlugin
    {
        //public static ConfigEntry<int> nexusID;
        //public static ConfigEntry<bool> modEnabled;
        //public static ConfigEntry<string> modKey;
        //public static ConfigEntry<int> repairDuration;

        //private static RepairWardPlugin self;

        private void Awake()
        {
            Debug.Log("Repair Ward Mod Loaded.");
            //self = this;

            //nexusID = Config.Bind<int>("General", "NexusID", , "NexusMods ID for updates");
            //modEnabled = Config.Bind<bool>("General", "Enabled", true, "Enable the mod");
            //modKey = Config.Bind<string>("General", "ModifierKey", "left shift", "Modifier key to trigger the repair function of a ward. Find valid keycodes here: https://docs.unity3d.com/Manual/ConventionalGameInput.html");
            //repairDuration = Config.Bind<int>("General", "RepairDuration", 10, "Number of seconds to fully repair warded objects. Use 0 for instant repair.");

            //if (!modEnabled.Value)
            //    return;

            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            //WayStone w = new WayStone();
        }
        
        // [HarmonyPatch(typeof(WayStone))]
    }
}

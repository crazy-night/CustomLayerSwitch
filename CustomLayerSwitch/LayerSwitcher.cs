using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Studio;
using AIChara;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace StudioCustomLayerSwitcher
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency("marco.kkapi", "1.4")]
    [BepInProcess("StudioNEOV2.exe")]
    public class LayerSwitcher : BaseUnityPlugin
    {
        public const string GUID = "PhoeniX.SCLS";
        public const string Name = "Studio Custom Layer Switcher";
        public const string Version = "1.0";
        internal new static ManualLogSource Logger;

        public static LayerSwitcher Instance { get; private set; }
        public static ConfigEntry<KeyboardShortcut> KeyShowUI { get; private set; }

        public static ConfigEntry<bool> VerboseMessage { get; private set; }

        public static ConfigEntry<int> UIXPosition { get; private set; }

        public static ConfigEntry<int> UIYPosition { get; private set; }

        public static ConfigEntry<int> UIWidth { get; private set; }

        public static ConfigEntry<int> UIHeight { get; private set; }
        public static ConfigEntry<string> UILanguage { get; private set; }
        private void Awake()
        {
            LayerSwitcher.Instance = this;
            LayerSwitcher.Logger = base.Logger;
            if (!KKAPI.Studio.StudioAPI.InsideStudio) return;

            LayerSwitcher.KeyShowUI = base.Config.Bind<KeyboardShortcut>("General", "LayerSwitcher UI shortcut key", new KeyboardShortcut(KeyCode.L, new KeyCode[]
            {
                    KeyCode.LeftShift
            }), "Toggles the main UI on and off.");


            LayerSwitcher.VerboseMessage = base.Config.Bind<bool>("Debug", "Print verbose info", false, "Print more debug info to console.");
            LayerSwitcher.UIXPosition = base.Config.Bind<int>("GUI", "Main GUI X position", 50, "X offset from left in pixel");
            LayerSwitcher.UIYPosition = base.Config.Bind<int>("GUI", "Main GUI Y position", 300, "Y offset from top in pixel");
            LayerSwitcher.UIWidth = base.Config.Bind<int>("GUI", "Main GUI window width", 600, "Main window width, minimum 600, set it when UI is hided.");
            LayerSwitcher.UIHeight = base.Config.Bind<int>("GUI", "Main GUI window height", 400, "Main window height, minimum 400, set it when UI is hided.");
            LayerSwitcher.UILanguage = base.Config.Bind<string>("GUI", "GUI Language", "default", "Language setting, valid setting can be found in HS2LayerSwitch.xml. Need reload.");
            GameObject gameObject = new GameObject("Layer Switch");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            LayerSwitcherMgr.Install(gameObject);

        }
        

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaControl), "AssignCoordinate")]
        public static void ChaControl_AssignCoordinatePostfix(ChaControl __instance)
        {
            bool verbose= LayerSwitcher.VerboseMessage.Value;
            if (verbose) 
            {
                Console.WriteLine("ChaControl_AssignCoordinatePostfix: Start");
            }
            LayerSwitcherMgr.UpdateDict(__instance);
            // Add your postfix code here
        }
    }
}


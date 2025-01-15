using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using AIChara;
using UnityEngine;
using KKAPI.Studio.SaveLoad;

namespace StudioCustomLayerSwitcher
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency("Countd360.StudioCharaEditor.HS2")]
    [BepInProcess("StudioNEOV2.exe")]
    public class LayerSwitcher : BaseUnityPlugin
    {
        public const string GUID = "PhoeniX.StudioCustomLayerSwitcher";
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
        //public static ConfigEntry<string> UILanguage { get; private set; }
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
            //LayerSwitcher.UILanguage = base.Config.Bind<string>("GUI", "GUI Language", "default", "Language setting, valid setting can be found in HS2LayerSwitch.xml. Need reload.");


            Harmony.CreateAndPatchAll(typeof(LayerSwitcherHook), GUID);

            StudioSaveLoadApi.RegisterExtraBehaviour<LoadLog>(GUID);

            GameObject gameObject = new GameObject("Layer Switcher");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            LayerSwitcherMgr.Install(gameObject);

        }

        public static void Debug(string _text)
        {
            bool verbose = LayerSwitcher.VerboseMessage.Value;
            if (verbose)
            {
                //Console.WriteLine(_text);
                LayerSwitcher.Logger.LogDebug(_text);
            }
        }
        public static void Warning(string _text)
        {
            bool verbose = LayerSwitcher.VerboseMessage.Value;
            if (verbose)
            {
                LayerSwitcher.Logger.LogWarning(_text);
            }
        }
    }
}


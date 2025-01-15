using HarmonyLib;
using AIChara;
using System;
using System.Collections;

namespace StudioCustomLayerSwitcher
{
    public class LayerSwitcherHook
    {
        public static readonly string Name = "Studio Custom Layer Switcher";

        /*[HarmonyPostfix]
        [HarmonyPatch(typeof(ChaControl), "ReloadAsync")]
        public static IEnumerator SCLS_ReloadAsyncPostfix(IEnumerator __result, ChaControl __instance)
        {
            LayerSwitcher.Debug(Name + ".ReloadAsyncPostfix: Start");
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
            LayerSwitcherMgr.UpdateDict(__instance);
            LayerSwitcher.Debug(Name + ".ReloadAsyncPostfix: End");
            yield break;
            // Add your postfix code here
        }*/

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaControl), "ChangeClothesAsync", new Type[] { typeof(int), typeof(int), typeof(bool), typeof(bool) })]
        public static IEnumerator SCLS_ChangeClothesAsync_Postfix(IEnumerator __result, ChaControl __instance)
        {
            LayerSwitcher.Debug(Name + ".ChangeClothesAsync_Postfix: Start");
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
            LayerSwitcherMgr.MaintainDict(__instance, 0);
            LayerSwitcher.Debug(Name + ".ChangeClothesAsync_Postfix: End");
            yield break;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChaControl), "ChangeAccessoryAsync", new Type[] { typeof(int), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(bool) })]
        public static IEnumerator SCLS_ChangeAccessoryAsync_Postfix(IEnumerator __result, ChaControl __instance)
        {
            LayerSwitcher.Debug(Name + ".ChangeAccessoryAsync_Postfix: Start");
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
            LayerSwitcherMgr.MaintainDict(__instance, 1);
            LayerSwitcher.Debug(Name + ".ChangeAccessoryAsync_Postfix: End");
            yield break;
        }


    }
}

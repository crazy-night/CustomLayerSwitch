using BepInEx;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensibleSaveFormat;
using UnityEngine;
using BepInEx.Logging;

namespace StudioCustomLayerSwitch
{
    [BepInProcess("StudioNEOV2")]
    [BepInPlugin("SCLS.LoadLog", "LoadLog", "0.0.2")]
    public class LoadLog : BaseUnityPlugin
    {
        // Token: 0x0600000D RID: 13 RVA: 0x00002A78 File Offset: 0x00000C78
        public void Awake()
        {
            LoadLog.Debug("Loading Chara Clothes Layer!");
            ExtendedSave.SceneBeingLoaded += LoadLog.ExtendedSceneLoad;
            ExtendedSave.SceneBeingImported += LoadLog.ExtendedSceneImport;
            ExtendedSave.SceneBeingSaved += LoadLog.ExtendedSceneSave;
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002068 File Offset: 0x00000268
        public static void Debug(string _text)
        {
            BepInEx.Logging.Logger.CreateLogSource("SCLS").Log(LogLevel.Info, _text);
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00002AC8 File Offset: 0x00000CC8
        internal static void ExtendedSceneSave(string path)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<StudioLayerResolveInfo> list = new List<StudioLayerResolveInfo>();
            Dictionary<int, ObjectCtrlInfo> dicObjectCtrl = Singleton<Studio.Studio>.Instance.dicObjectCtrl;
            foreach (ObjectCtrlInfo objectCtrlInfo in dicObjectCtrl.Values)
            {
                if (objectCtrlInfo is OCIChar)
                {
                    OCIChar ociTarget = (OCIChar)objectCtrlInfo;
                    if (ociTarget != null)
                    {
                        CharaLayerController charaLayerController = new CharaLayerController(ociTarget);
                        StudioLayerResolveInfo studioLayerResolveInfo = new StudioLayerResolveInfo();
                        studioLayerResolveInfo.dicKey = ociTarget.objectInfo.dicKey;
                        foreach (int layer in charaLayerController.layers)
                        {
                            studioLayerResolveInfo.layers.Add(layer);
                            list.Add(studioLayerResolveInfo);
                        }
                        string text = string.Concat(new string[]
                        {   "SaveClothesLayerInfo!dickey:",
                            studioLayerResolveInfo.dicKey.ToString()
                        });
                        LoadLog.Debug(text);
                    }
                }
            }
            if (list != null)
            {
                if (list.Count != 0)
                {
                    dictionary.Add("ClothesLayerInfo", (from x in list
                                                        select x.Serialize()).ToList<byte[]>());
                }
            }
            if (dictionary.Count == 0)
            {
                ExtendedSave.SetSceneExtendedDataById("Studio Clothes Layers", null);
            }
            else
            {
                ExtendedSave.SetSceneExtendedDataById("Studio Clothes Layers", new PluginData
                {
                    data = dictionary
                });
            }
        }

        // Token: 0x06000010 RID: 16 RVA: 0x0000207E File Offset: 0x0000027E
        internal static void ExtendedSceneLoad(string path)
        {
            Singleton<Studio.Studio>.Instance.DelayToDo(0.5f, delegate
            {
                LoadLog.OnSceneLoad();
            }, false);
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002FC8 File Offset: 0x000011C8
        internal static void OnSceneLoad()
        {
            LoadInfo(0);
        }

        // Token: 0x06000012 RID: 18 RVA: 0x000020B1 File Offset: 0x000002B1
        internal static void ExtendedSceneImport(string path)
        {
            Singleton<Studio.Studio>.Instance.DelayToDo(0.5f, delegate
            {
                LoadLog.OnSceneImport();
            }, false);
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00003414 File Offset: 0x00001614
        internal static void OnSceneImport()
        {
            Dictionary<int, ObjectInfo> dicObject = Singleton<Studio.Studio>.Instance.sceneInfo.dicObject;
            Dictionary<int, ObjectInfo> dicImport = Singleton<Studio.Studio>.Instance.sceneInfo.dicImport;
            int offset = dicObject.Count - dicImport.Count;
            LoadInfo(offset);
        }
        private static void LoadInfo(int offset)
        {
            PluginData sceneExtendedDataById = ExtendedSave.GetSceneExtendedDataById("Studio Clothes Layers");
            Dictionary<int, ObjectCtrlInfo> dicObjectCtrl = Singleton<Studio.Studio>.Instance.dicObjectCtrl;
            if (sceneExtendedDataById != null && sceneExtendedDataById.data.ContainsKey("ClothesLayerInfo"))
            {
                List<StudioLayerResolveInfo> list = (from x in (object[])sceneExtendedDataById.data["ClothesLayerInfo"]
                                                     select StudioLayerResolveInfo.Deserialize((byte[])x)).ToList<StudioLayerResolveInfo>();
                foreach (StudioLayerResolveInfo StudioLayerResolveInfo in list)
                {
                    ObjectCtrlInfo objectCtrlInfo = dicObjectCtrl[StudioLayerResolveInfo.dicKey + offset];
                    if (objectCtrlInfo is OCIChar)
                    {
                        OCIChar ociTarget = (OCIChar)objectCtrlInfo;
                        if (ociTarget != null)
                        {
                            CharaLayerController charaLayerController = new CharaLayerController(ociTarget);
                            int i = 0;
                            foreach (var cloth in charaLayerController.clothes)
                            {
                                int layer = StudioLayerResolveInfo.layers[i];
                                if (layer != 0)
                                {
                                    foreach (var gameObject in cloth)
                                    {
                                        gameObject.layer = layer;
                                    }
                                }
                                ++i;
                            }
                        }
                    }
                }
            }
        }

    }
}

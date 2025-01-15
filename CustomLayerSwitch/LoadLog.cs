using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using ExtensibleSaveFormat;
using KKAPI.Studio;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using AIChara;

namespace StudioCustomLayerSwitcher
{
    public class LoadLog : SceneCustomFunctionController
    {
        private readonly string infoKey = "LayerInfo";

        public static void Debug(string _text)
        {
            LayerSwitcher.Debug(_text);
        }

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            if (operation == SceneOperationKind.Clear)
            {
                LayerSwitcherMgr.Clear();
                return;
            }
            else if (operation == SceneOperationKind.Load)
            {
                LayerSwitcherMgr.Clear();
            }
            PluginData sceneExtendedData = GetExtendedData();
            if (sceneExtendedData != null && sceneExtendedData.data.ContainsKey(infoKey))
            {
                List<StudioLayerResolveInfo> list = (from x in (object[])sceneExtendedData.data[infoKey]
                                                     select StudioLayerResolveInfo.Deserialize((byte[])x)).ToList<StudioLayerResolveInfo>();

                foreach (StudioLayerResolveInfo studioLayerResolveInfo in list)
                {
                    if (loadedItems.TryGetValue(studioLayerResolveInfo.dicKey, out ObjectCtrlInfo objectCtrlInfo) && objectCtrlInfo is OCIChar ociChar)
                    {

                        CharaLayerController charaLayerController = LayerSwitcherMgr.GetCharaLayerController(ociChar);
                        int i = 0;
                        foreach (int layer in studioLayerResolveInfo.clothesLayers)
                        {
                            charaLayerController.SetClothesLayer(i++, layer);
                        }
                        LoadLog.Debug($"Studio Custom Layer Switch: Clothes Layers of {ociChar.charInfo.name} Reload Successfully!");

                        i = 0;
                        foreach (int layer in studioLayerResolveInfo.accessoryLayers)
                        {
                            charaLayerController.SetAccessoryLayer(i++, layer);
                        }
                        LoadLog.Debug($"Studio Custom Layer Switch: Accessory Layers of {ociChar.charInfo.name} Reload Successfully!");
                    }
                    else
                    {
                        throw new Exception("Studio Custom Layer Switch: Failed to find character!");
                    }

                }
            }
        }

        protected override void OnSceneSave()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<StudioLayerResolveInfo> list = new List<StudioLayerResolveInfo>();
            foreach (ChaControl charInfo in LayerSwitcherMgr.charaLayerCtrlDict.Keys)
            {
                OCIChar ociChar = LayerSwitcherMgr.charaDict[charInfo];
                CharaLayerController charaLayerController = LayerSwitcherMgr.GetCharaLayerController(ociChar);
                if (charaLayerController.HasChanged)
                {
                    StudioLayerResolveInfo studioLayerResolveInfo = new StudioLayerResolveInfo
                    {
                        dicKey = ociChar.oiCharInfo.dicKey
                    };

                    foreach (int layer in charaLayerController.ClothesLayers)
                    {
                        studioLayerResolveInfo.clothesLayers.Add(layer);
                    }
                    foreach (int layer in charaLayerController.AccessoryLayers)
                    {
                        studioLayerResolveInfo.accessoryLayers.Add(layer);
                    }
                    list.Add(studioLayerResolveInfo);
                    LoadLog.Debug("SaveLayerInfo!dickey:" + studioLayerResolveInfo.dicKey.ToString());
                }

            }
            if (list != null && list.Count > 0)
            {
                dictionary.Add(infoKey, (from x in list
                                         select x.Serialize()).ToList<byte[]>());

            }
            SetExtendedData(dictionary.Count > 0 ? new PluginData { data = dictionary } : null);
        }
        protected override void OnObjectDeleted(ObjectCtrlInfo objectCtrlInfo)
        {
            if (objectCtrlInfo is OCIChar ociChar)
            {
                LayerSwitcherMgr.RemoveDict(ociChar.charInfo);
            }
        }
        protected override void OnObjectsCopied(ReadOnlyDictionary<int, ObjectCtrlInfo> copiedItems)
        {
            foreach (KeyValuePair<int, ObjectCtrlInfo> kvp in copiedItems)
            {
                if (kvp.Value is OCIChar ociDST)
                {
                    if (GetStudio().dicObjectCtrl.TryGetValue(kvp.Key, out ObjectCtrlInfo oci))
                    {
                        OCIChar ociSRC = oci as OCIChar;
                        if (LayerSwitcherMgr.charaLayerCtrlDict.ContainsKey(ociSRC.charInfo))
                        {
                            CharaLayerController charaLayerControllerSRC = LayerSwitcherMgr.GetCharaLayerController(ociSRC);
                            if (charaLayerControllerSRC.HasChanged)
                            {
                                CharaLayerController charaLayerControllerDST = LayerSwitcherMgr.GetCharaLayerController(ociDST);
                                charaLayerControllerDST.Copy(charaLayerControllerSRC);
                            }
                        }
                    }
                }
            }

        }
    }
}

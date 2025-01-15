using AIChara;
using StudioCharaEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using HarmonyLib;

namespace StudioCustomLayerSwitcher
{
    internal class CharaLayerController
    {
        static private readonly string[] MALE_CLOTHES_NAME = { "Top", "Bot", "Gloves", "Shoes" };
        static private readonly string[] FEMALE_CLOTHES_NAME = { "Top", "Bot", "Inner_t", "Inner_b", "Gloves", "Panst", "Socks", "Shoes" };
        internal int[] ClothesLayers { get; private set; }
        internal List<GameObject>[] clothes;
        private bool isMale;
        internal string name;
        internal string[] ACCESSORY_NAME;
        internal int[] AccessoryLayers { get; private set; }
        internal List<GameObject>[] accessories;
        public bool HasChanged
        {
            get
            {
                return ClothesLayers.Any(x => x != 10 && x != 0) || AccessoryLayers.Any(x => x != 10 && x != 0);
            }
        }

        internal string[] ClothesNames
        {
            get
            { return isMale ? MALE_CLOTHES_NAME : FEMALE_CLOTHES_NAME; }
        }
        internal string[] AccessoryNames
        {
            get
            { return ACCESSORY_NAME; }
        }
        internal CharaLayerController(ChaControl charInfo)
        {
            Update(charInfo);
        }
        internal void Update(ChaControl charInfo)
        {
            if (charInfo == null)
            {
                LayerSwitcher.Debug($"Error! CharaLayerController.Update: CharInfo is null.");
                return;
            }
            isMale = charInfo.sex == 0;
            name = charInfo.name;
            UpdateClothes(charInfo);
            UpdateAccessory(charInfo);
        }
        internal void Copy(CharaLayerController source)
        {
            CopyClothesLayer(source);
            CopyAccessoryLayer(source);
            LayerSwitcher.Debug($"Studio Custom Layer Switch: Copy Layer from {source.name} to {name} Successfully!");
        }

        internal void Maintain(ChaControl charInfo, int category)
        {
            if (charInfo == null)
            {
                LayerSwitcher.Debug($"Error! CharaLayerController.Maintain: CharInfo is null.");
                return;
            }
            if (category == 0)
            {
                MaintainClothesLayer(charInfo);
            }
            else if (category == 1)
            {
                MaintainAccessoryLayer(charInfo);
            }
            LayerSwitcher.Debug($"Studio Custom Layer Switch: Maintain Layers of {name} Successfully!");
        }

        internal void UpdateClothes(ChaControl charInfo)
        {
            int index = charInfo.cmpClothes.Length;
            clothes = new List<GameObject>[index];
            ClothesLayers = new int[index];
            for (int i = 0; i < index; ++i)
            {
                CmpClothes cmpClothes = charInfo.cmpClothes[i];
                List<GameObject> targets = new List<GameObject>();
                if (cmpClothes != null)
                {
                    GetRenderObjects(cmpClothes.rendNormal01, targets);
                    GetRenderObjects(cmpClothes.rendNormal02, targets);
                    GetRenderObjects(cmpClothes.rendNormal03, targets);
                }

                clothes[i] = targets;

                ClothesLayers[i] = targets.Count > 0 ? targets.First().layer : 0;
            }

            LayerSwitcher.Debug($"CharaLayerController: {charInfo.name} has {clothes.Length} clothes objects and {ClothesLayers.Length} clothesLayers.");

        }

        internal void UpdateAccessory(ChaControl charInfo)
        {
            int index = PluginMoreAccessories.GetAccessoryCount(charInfo);

            ACCESSORY_NAME = new string[index];
            for (int i = 0; i < index; i++)
            {
                ListInfoBase info = PluginMoreAccessories.GetAccessoryInfo(charInfo, i);
                if (info != null)
                {
                    ACCESSORY_NAME[i] = info.Name;
                }
                else
                {
                    ACCESSORY_NAME[i] = null;
                }
            }

            accessories = new List<GameObject>[index];
            AccessoryLayers = new int[index];
            for (int i = 0; i < index; i++)
            {
                CmpAccessory cmpAccessory = PluginMoreAccessories.GetAccessoryCmp(charInfo, i);
                List<GameObject> targets = new List<GameObject>();
                if (cmpAccessory != null)
                {
                    GetRenderObjects(cmpAccessory.rendNormal, targets);
                    GetRenderObjects(cmpAccessory.rendAlpha, targets);
                }
                accessories[i] = targets;
                AccessoryLayers[i] = targets.Count > 0 ? targets.First().layer : 0;
            }

            LayerSwitcher.Debug($"CharaLayerController: {charInfo.name} has {accessories.Length} accessories objects and {AccessoryLayers.Length} accessoryLayers.");

        }

        private void GetRenderObjects(Renderer[] renderers, List<GameObject> list, int layer = 0)
        {

            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    if (layer != 0 && layer != renderer.gameObject.layer)
                    {
                        renderer.gameObject.layer = layer;
                    }
                    list.Add(renderer.gameObject);
                }
            }

        }
        internal void SetClothesLayer(int i, int layer)
        {
            if (i >= ClothesLayers.Length)
            {
                LayerSwitcher.Debug($"Error! CharaLayerController.SetClothesLayer: Index out of range.");
                return;
            }
            if (layer != 9 && layer != 10 && layer != 14)
            {
                return;
            }
            if (ClothesLayers[i] != 0 && ClothesLayers[i] != layer)
            {
                foreach (var gameObject in clothes[i])
                {
                    if (gameObject != null)
                    {
                        gameObject.layer = layer;
                    }
                    else
                    {
                        LayerSwitcher.Debug($"Error! CharaLayerController.SetClothesLayer: {ClothesNames[i]} is null.");
                    }
                }
                ClothesLayers[i] = layer;
                LayerSwitcher.Debug($"CharaLayerCtrl.SetClothesLayer: {name}'s {ClothesNames[i]} layer set to {layer}.");
            }
        }

        internal void SetAccessoryLayer(int i, int layer)
        {
            if (i >= AccessoryLayers.Length)
            {
                LayerSwitcher.Debug($"Error! CharaLayerCtrl.SetAccessoryLayer: Index out of range.");
                return;
            }
            if (layer != 9 && layer != 10 && layer != 14)
            {
                return;
            }
            if (AccessoryLayers[i] != 0 && AccessoryLayers[i] != layer)
            {
                foreach (var gameObject in accessories[i])
                {
                    if (gameObject != null)
                    {
                        gameObject.layer = layer;
                    }
                    else
                    {
                        LayerSwitcher.Debug($"Error! CharaLayerController.SetAccessoryLayer: {AccessoryNames[i]} of {name} is null.");
                    }
                }
                AccessoryLayers[i] = layer;
                LayerSwitcher.Debug($"CharaLayerCtrl.SetAccessoryLayer: {name}'s {AccessoryNames[i]} layer set to {layer}.");
            }
        }
        internal void CopyClothesLayer(CharaLayerController source)
        {
            for (int i = 0; i < ClothesLayers.Length; i++)
            {
                SetClothesLayer(i, source.ClothesLayers[i]);
            }
        }
        internal void CopyAccessoryLayer(CharaLayerController source)
        {
            for (int i = 0; i < AccessoryLayers.Length; i++)
            {
                SetAccessoryLayer(i, source.AccessoryLayers[i]);
            }
        }


        internal void MaintainClothesLayer(ChaControl charInfo)
        {
            int index = charInfo.cmpClothes.Length;
            clothes = new List<GameObject>[index];
            for (int i = 0; i < index; ++i)
            {
                CmpClothes cmpClothes = charInfo.cmpClothes[i];
                List<GameObject> targets = new List<GameObject>();
                if (cmpClothes != null)
                {
                    GetRenderObjects(cmpClothes.rendNormal01, targets, ClothesLayers[i]);
                    GetRenderObjects(cmpClothes.rendNormal02, targets, ClothesLayers[i]);
                    GetRenderObjects(cmpClothes.rendNormal03, targets, ClothesLayers[i]);
                }
                clothes[i] = targets;
                //clothesLayers[i] = targets.Count > 0 ? targets.First().layer : 0;
                LayerSwitcher.Debug("MaintainClothesLayer: " + name + " Keeps the clothes layers");

            }
        }

        internal void MaintainAccessoryLayer(ChaControl charInfo)
        {
            int index = PluginMoreAccessories.GetAccessoryCount(charInfo);
            accessories = new List<GameObject>[index];
            for (int i = 0; i < index; i++)
            {
                CmpAccessory cmpAccessory = PluginMoreAccessories.GetAccessoryCmp(charInfo, i);
                List<GameObject> targets = new List<GameObject>();
                if (cmpAccessory != null)
                {
                    GetRenderObjects(cmpAccessory.rendNormal, targets, AccessoryLayers[i]);
                    GetRenderObjects(cmpAccessory.rendAlpha, targets, AccessoryLayers[i]);
                }
                accessories[i] = targets;
                AccessoryLayers[i] = targets.Count > 0 ? targets.First().layer : 0;
                LayerSwitcher.Debug("MaintainAccessoryLayer: " + name + " Keeps the accessory layers");
            }
        }

    }
}

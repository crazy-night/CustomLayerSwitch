using AIChara;
using AIProject.Animal;
using Studio;
using StudioCustomLayerSwitcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace StudioCustomLayerSwitch
{
    internal class CharaLayerController
    {
        internal List<int> layers { get; set; }
        internal List<List<GameObject>> clothes;
        internal bool isMale { get; set; }
        internal CharaLayerController(OCIChar chara)
        {
            Update(chara);
        }
        internal void Update(OCIChar chara)
        {
            isMale = chara.charInfo.sex == 0;
            clothes = new List<List<GameObject>>();
            layers = new List<int>();
            foreach (GameObject charaClothes in chara.charInfo.objClothes)
            {
                clothes.Add(GetClothesObjects(charaClothes));
                if (clothes.Back().Count == 0)
                {
                    layers.Add(0);
                }
                else
                {
                    layers.Add(clothes.Back().Back().layer);
                }
            }
            bool verbose = LayerSwitcher.VerboseMessage.Value;
            if (verbose)
            {
                Console.WriteLine($"CharaLayerController: Detection Successfully!\n{chara.charInfo.name} has {clothes.Count} clothes objects and {layers.Count} layers.");
                LayerSwitcher.Logger.LogInfo($"CharaLayerController: Detection Successfully!\n{chara.charInfo.name} has {clothes.Count} clothes objects and {layers.Count} layers.");
            }
        }
        internal void Update(int i,int layer)
        {
            layers[i] = layer;
        }
        private List<GameObject> GetClothesObjects(GameObject gameObject)
        {

            Stack<GameObject> stack = new Stack<GameObject>();
            List<GameObject> targets = new List<GameObject>();
            if (gameObject == null || gameObject.transform.childCount == 0) return targets;

            stack.Push(gameObject);
            while (stack.Count > 0)
            {
                GameObject obj = stack.Pop();
                int childCount = obj.transform.childCount;
                if (childCount > 0)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        stack.Push(obj.transform.GetChild(i).gameObject);
                    }
                }
                else
                {
                    targets.Add(obj);
                }
            }
            return targets;
        }
    }
}

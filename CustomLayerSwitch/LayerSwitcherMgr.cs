using AIChara;
using HS2;
using Studio;
using StudioCustomLayerSwitch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StudioCustomLayerSwitcher
{
    internal class LayerSwitcherMgr : MonoBehaviour
    {
        public static LayerSwitcherMgr Instance { get; private set; }
        static internal Dictionary<OCIChar, CharaLayerController> charaLayerCtrlDict = new Dictionary<OCIChar, CharaLayerController>();
        public static LayerSwitcherMgr Install(GameObject container)
        {
            if (LayerSwitcherMgr.Instance == null)
            {
                LayerSwitcherMgr.Instance = container.AddComponent<LayerSwitcherMgr>();
            }
            return LayerSwitcherMgr.Instance;
        }

        public bool VisibleGUI
        {
            get
            {
                return this.gui.VisibleGUI;
            }
            set
            {
                this.gui.VisibleGUI = value;
            }
        }

        private void Awake()
        {
        }
        private void Start()
        {
            base.StartCoroutine(this.LoadingCo());
        }

        private IEnumerator LoadingCo()
        {
            yield return new WaitUntil(() => KKAPI.Studio.StudioAPI.StudioLoaded);
            yield return null;
            this.gui = new GameObject("GUI").AddComponent<LayerSwitcherUI>();
            this.gui.transform.parent = base.transform;
            this.gui.VisibleGUI = false;
            yield break;
        }
        
        public void ResetGUI()
        {
            this.gui.ResetGui();
        }
        public void HouseKeeping(bool isVisible)
        {
            // release deleted controller
            if (isVisible)
            {
                foreach (OCIChar ociChar in charaLayerCtrlDict.Keys)
                {
                    if (ociChar.charInfo == null)
                    {
                        Console.WriteLine("Remove controller for deleted chara");
                        charaLayerCtrlDict.Remove(ociChar);
                        return;
                    }
                }
            }
        }
        internal CharaLayerController GetCharaLayerController(OCIChar ociTarget)
        {
            if (ociTarget == null)
            {
                return null;
            }
            if (!charaLayerCtrlDict.ContainsKey(ociTarget))
            {
                charaLayerCtrlDict[ociTarget] = new CharaLayerController(ociTarget);
            }
            return charaLayerCtrlDict[ociTarget];
        }

        static public void UpdateDict(OCIChar _ociTarget)
        {
            bool verbose = LayerSwitcher.VerboseMessage.Value;
            if (charaLayerCtrlDict.ContainsKey(_ociTarget))
            {
                if (verbose)
                {
                    Console.WriteLine("UpdateDict: Update CharaLayerController");
                }
                charaLayerCtrlDict[_ociTarget].Update(_ociTarget);
            }
            else
            {
                if(verbose)
                {
                    Console.WriteLine("UpdateDict: Not existed CharaLayerController");
                }
            }
        }


        public LayerSwitcherUI gui;

    }
}

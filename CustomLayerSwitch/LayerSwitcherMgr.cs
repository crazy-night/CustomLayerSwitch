using AIChara;
using Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StudioCustomLayerSwitcher
{
    internal class LayerSwitcherMgr : MonoBehaviour
    {
        public static LayerSwitcherMgr Instance { get; private set; }
        static internal Dictionary<ChaControl, CharaLayerController> charaLayerCtrlDict = new Dictionary<ChaControl, CharaLayerController>();
        static internal Dictionary<ChaControl, OCIChar> charaDict = new Dictionary<ChaControl, OCIChar>();
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

        internal static CharaLayerController GetCharaLayerController(OCIChar ociTarget)
        {
            if (ociTarget == null || ociTarget.charInfo == null)
            {
                return null;
            }
            if (!charaLayerCtrlDict.ContainsKey(ociTarget.charInfo))
            {
                charaLayerCtrlDict[ociTarget.charInfo] = new CharaLayerController(ociTarget.charInfo);
                charaDict[ociTarget.charInfo] = ociTarget;
                LayerSwitcher.Debug("GetCharaLayerController: New CharaLayerController");
            }
            return charaLayerCtrlDict[ociTarget.charInfo];
        }

        internal static void Clear()
        {
            charaLayerCtrlDict.Clear();
            charaDict.Clear();
        }

        internal static void RemoveDict(ChaControl charInfo)
        {
            if (charaLayerCtrlDict.ContainsKey(charInfo))
            {
                charaLayerCtrlDict.Remove(charInfo);
                charaDict.Remove(charInfo);
            }
        }

        static internal void UpdateDict(ChaControl charInfo)
        {
            if (charaLayerCtrlDict.ContainsKey(charInfo))
            {
                LayerSwitcher.Debug("UpdateDict: Existed CharaLayerController");
                charaLayerCtrlDict[charInfo].Update(charInfo);
            }
            else
            {
                LayerSwitcher.Debug("Error!UpdateDict: Not existed CharaLayerController");
            }
        }

        static internal void MaintainDict(ChaControl charInfo, int category)
        {
            if (charaLayerCtrlDict.ContainsKey(charInfo))
            {
                LayerSwitcher.Debug("MaintainDict: Existed CharaLayerController");
                charaLayerCtrlDict[charInfo].Maintain(charInfo, category);
            }
            else
            {
                LayerSwitcher.Debug("Error!MaintainDict: Not existed CharaLayerController");
            }
        }


        public LayerSwitcherUI gui;

    }
}

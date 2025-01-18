using Studio;
using System;
using System.Collections.Generic;
using UnityEngine;
using KKAPI.Utilities;


namespace StudioCustomLayerSwitcher
{
    internal class LayerSwitcherUI : MonoBehaviour
    {


        static public readonly Dictionary<int, string> LAYERTAGS = new Dictionary<int, string>()
        {
            { 14, "HideInPlane" },
            { 10, "Normal" },
            { 9, "HideInMain" },
            { 0, "None" }
        };
        static public readonly int[] LAYERINDEXES = { 14, 10, 9 };
        static public readonly string[] CATEGORY = { "Clothes", "Accessories" };
        public enum Category { Clothes = 0, Accessories = 1 };
        public Category category = Category.Clothes;
        public int layerIndex = 14;
        public CharaLayerController charaLayerCtrl;
        private Vector2 scrollPosition;

        public bool VisibleGUI { get; set; }

        // Token: 0x06000093 RID: 147 RVA: 0x00002439 File Offset: 0x00000639
        public void ResetGui()
        {
            this.ociTarget = null;
            this.layerIndex = 14;
            this.category = 0;
        }

        // Token: 0x06000094 RID: 148 RVA: 0x00004CFC File Offset: 0x00002EFC
        private void Start()
        {
            this.largeLabel = new GUIStyle("label");
            this.largeLabel.fontSize = 16;
            this.btnstyle = new GUIStyle("button");
            this.btnstyle.fontSize = 16;
        }

        // Token: 0x06000095 RID: 149 RVA: 0x00004D50 File Offset: 0x00002F50
        private void Update()
        {
            if (LayerSwitcher.KeyShowUI.Value.IsDown())
            {
                this.VisibleGUI = !this.VisibleGUI;
                if (this.VisibleGUI)
                {
                    this.windowRect = new Rect((float)LayerSwitcher.UIXPosition.Value, (float)LayerSwitcher.UIYPosition.Value, (float)Math.Max(600, LayerSwitcher.UIWidth.Value), (float)Math.Max(400, LayerSwitcher.UIHeight.Value));
                }
                else
                {
                    LayerSwitcher.UIXPosition.Value = (int)this.windowRect.x;
                    LayerSwitcher.UIYPosition.Value = (int)this.windowRect.y;
                    LayerSwitcher.UIWidth.Value = (int)this.windowRect.width;
                    LayerSwitcher.UIHeight.Value = (int)this.windowRect.height;
                }
            }
            if (this.VisibleGUI)
            {
                TreeNodeObject currentSelectedNode = this.GetCurrentSelectedNode();
                if (currentSelectedNode != this.lastSelectedTreeNode)
                {
                    this.OnSelectChange(currentSelectedNode);
                }
            }
        }

        // Token: 0x06000096 RID: 150 RVA: 0x00004E54 File Offset: 0x00003054
        private void OnGUI()
        {
            if (this.VisibleGUI)
            {
                try
                {
                    GUIStyle style = new GUIStyle(GUI.skin.window);
                    this.windowRect = GUI.Window(this.windowID, this.windowRect, new GUI.WindowFunction(this.FuncWindowGUI), this.windowTitle, style);
                    this.mouseInWindow = this.windowRect.Contains(Event.current.mousePosition);
                    if (this.mouseInWindow)
                    {
                        Singleton<Studio.Studio>.Instance.cameraCtrl.noCtrlCondition = (() => this.mouseInWindow && this.VisibleGUI);
                        Input.ResetInputAxes();
                    }
                }
                catch (Exception value)
                {
                    Console.WriteLine(value);
                }
            }
        }

        // Token: 0x06000097 RID: 151 RVA: 0x00004F04 File Offset: 0x00003104
        private void FuncWindowGUI(int winID)
        {
            try
            {
                int hotControl = GUIUtility.hotControl;
                if (Event.current.type == EventType.MouseDown)
                {
                    GUI.FocusControl("");
                    GUI.FocusWindow(winID);
                }
                guiEditorMain();
                GUI.enabled = true;
                GUI.DragWindow();
            }
            catch (Exception value)
            {
                Console.WriteLine(value);
                this.ResetGui();
            }
        }
        private void guiEditorMain()
        {
            float fullw = windowRect.width - 20;
            float fullh = windowRect.height - 20;
            float toph = fullh - 200;
            float leftw = 180;
            float rightw = fullw - 10 - leftw - 28;
            charaLayerCtrl = LayerSwitcherMgr.GetCharaLayerController(ociTarget);

            if (ociTarget == null || charaLayerCtrl == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("<color=#00ffff>" + "Please select a charactor to edit." + "</color>", largeLabel,IMGUIUtils.EmptyLayoutOptions);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Current LayerIndex Setting: " + charaLayerCtrl.name, largeLabel,IMGUIUtils.EmptyLayoutOptions);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                for (int i = 0; i < 3; i++)
                {
                    Color color = GUI.color;
                    if (layerIndex == LAYERINDEXES[i])
                    {
                        GUI.color = Color.red;
                    }
                    if (GUILayout.Button(LAYERTAGS[LAYERINDEXES[i]], btnstyle,IMGUIUtils.EmptyLayoutOptions))
                    {
                        layerIndex = LAYERINDEXES[i];
                    }
                    GUI.color = color;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                for (int i = 0; i < 2; i++)
                {
                    Color color = GUI.color;
                    if ((int)category == i)
                    {
                        GUI.color = Color.red;
                    }
                    if (GUILayout.Button(CATEGORY[i], btnstyle, IMGUIUtils.EmptyLayoutOptions))
                    {
                        category = (Category)i;
                    }
                    GUI.color = color;
                }
                GUILayout.EndHorizontal();

                if (category == Category.Clothes)
                {
                    int index = charaLayerCtrl.ClothesNames.Length;
                    bool[] click = new bool[8];
                    string[] clothesNames = charaLayerCtrl.ClothesNames;

                    GUILayout.BeginHorizontal();

                    GUILayout.BeginVertical(GUILayout.Width(leftw + 10));
                    foreach (string clothName in clothesNames)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(clothName, largeLabel, IMGUIUtils.EmptyLayoutOptions);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();


                    GUILayout.BeginVertical(GUILayout.Width(rightw));
                    for (int i = 0; i < index; ++i)
                    {
                        GUILayout.BeginHorizontal();
                        if (!LAYERTAGS.ContainsKey(charaLayerCtrl.ClothesLayers[i]))
                        {
                            GUILayout.Label("Layer " + charaLayerCtrl.ClothesLayers[i].ToString(), largeLabel, IMGUIUtils.EmptyLayoutOptions);
                        }
                        else
                        {
                            GUILayout.Label(LAYERTAGS[charaLayerCtrl.ClothesLayers[i]].ToString(), largeLabel, IMGUIUtils.EmptyLayoutOptions);
                        }
                        GUILayout.FlexibleSpace();
                        if (charaLayerCtrl.ClothesLayers[i] != 0)
                        {
                            click[i] = GUILayout.Button("Set Layer Index", btnstyle, IMGUIUtils.EmptyLayoutOptions);
                        }
                        else
                        {
                            GUILayout.Label("No Clothes", largeLabel, IMGUIUtils.EmptyLayoutOptions);
                            click[i] = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    bool flag = GUILayout.Button("Set All Layers", btnstyle, IMGUIUtils.EmptyLayoutOptions);
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < index; ++i)
                    {
                        if (flag || click[i])
                        {
                            charaLayerCtrl.SetClothesLayer(i, layerIndex);
                        }
                    }
                }
                else
                {
                    int index = charaLayerCtrl.ACCESSORY_NAME.Length;
                    bool[] click = new bool[index];
                    string[] accessoryNames = charaLayerCtrl.AccessoryNames;

                    scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(fullw), GUILayout.Height(toph));

                    GUILayout.BeginHorizontal();

                    GUILayout.BeginVertical(GUILayout.Width(leftw + 10));
                    foreach (string accessoryName in accessoryNames)
                    {
                        GUILayout.BeginHorizontal();
                        if (accessoryName == null)
                        {
                            GUILayout.Label("No Accessory", largeLabel, IMGUIUtils.EmptyLayoutOptions);
                        }
                        else
                        {
                            GUILayout.Label(accessoryName, largeLabel, IMGUIUtils.EmptyLayoutOptions);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();


                    GUILayout.BeginVertical(GUILayout.Width(rightw));
                    for (int i = 0; i < index; ++i)
                    {
                        GUILayout.BeginHorizontal();
                        if (!LAYERTAGS.ContainsKey(charaLayerCtrl.AccessoryLayers[i]))
                        {
                            GUILayout.Label("Layer " + charaLayerCtrl.AccessoryLayers[i].ToString(), largeLabel, IMGUIUtils.EmptyLayoutOptions);
                        }
                        else
                        {
                            GUILayout.Label(LAYERTAGS[charaLayerCtrl.AccessoryLayers[i]].ToString(), largeLabel, IMGUIUtils.EmptyLayoutOptions);
                        }
                        GUILayout.FlexibleSpace();
                        if (charaLayerCtrl.AccessoryLayers[i] != 0)
                        {
                            click[i] = GUILayout.Button("Set Layer Index", btnstyle, IMGUIUtils.EmptyLayoutOptions);
                        }
                        else
                        {
                            GUILayout.Label("No Accessory", largeLabel, IMGUIUtils.EmptyLayoutOptions);
                            click[i] = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();


                    GUILayout.BeginHorizontal();
                    bool flag = GUILayout.Button("Set All Layers", btnstyle, IMGUIUtils.EmptyLayoutOptions);
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < index; ++i)
                    {
                        if (flag || click[i])
                        {
                            charaLayerCtrl.SetAccessoryLayer(i, layerIndex);
                        }
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh", btnstyle, IMGUIUtils.EmptyLayoutOptions))
                {
                    LayerSwitcherMgr.UpdateDict(ociTarget.charInfo);
                }
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();


            }
            // close btn
            Rect cbRect = new Rect(windowRect.width - 16, 3, 13, 13);
            Color oldColor = GUI.color;
            GUI.color = Color.red;
            if (GUI.Button(cbRect, ""))
            {
                VisibleGUI = false;
            }
            GUI.color = oldColor;
        }

        // Token: 0x0600009B RID: 155 RVA: 0x0000246F File Offset: 0x0000066F
        private void OnSelectChange(TreeNodeObject newSel)
        {
            lastSelectedTreeNode = newSel;
            ociTarget = GetOCICharFromNode(newSel);
        }

        protected TreeNodeObject GetCurrentSelectedNode()
        {
            return Studio.Studio.Instance.treeNodeCtrl.selectNode;
        }

        protected OCIChar GetOCICharFromNode(TreeNodeObject node)
        {
            if (node == null) return null;

            var dic = Studio.Studio.Instance.dicInfo;
            if (dic.ContainsKey(node))
            {
                ObjectCtrlInfo oci = dic[node];
                if (oci is OCIChar)
                {
                    return oci as OCIChar;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        // Token: 0x04000081 RID: 129
        private readonly int windowID = 1314150679;

        // Token: 0x04000082 RID: 130
        private readonly string windowTitle = "Studio Layer Switcher";

        // Token: 0x04000083 RID: 131
        private Rect windowRect = new Rect(0f, 300f, 600f, 400f);

        // Token: 0x04000084 RID: 132
        private bool mouseInWindow;

        // Token: 0x04000085 RID: 133
        private GUIStyle largeLabel;

        // Token: 0x04000086 RID: 134
        private GUIStyle btnstyle;

        // Token: 0x04000087 RID: 135
        private OCIChar ociTarget;

        // Token: 0x04000088 RID: 136
        private TreeNodeObject lastSelectedTreeNode;

    }
}

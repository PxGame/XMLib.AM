/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/8 0:02:47
 */

using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// QuickButtonHandler
    /// </summary>
    public class QuickButtonHandler
    {
        public ActionEditorWindow win { get; set; }

        public Rect winRect = new Rect(0, 100, 150, 300);

        public void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(sceneView.position);

            winRect = GUILayout.Window(0, winRect, OnWindowFunc, "动作编辑工具");

            GUILayout.EndArea();
            Handles.EndGUI();

            UpdateKey();
        }

        public void OnGUI()
        {
            UpdateKey();
        }

        private void UpdateKey()
        {
            if (!win.setting.enableQuickKey)
            {
                return;
            }

            Event evt = Event.current;

            if (!evt.isKey || evt.type != UnityEngine.EventType.KeyDown)
            {
                return;
            }

            switch (evt.keyCode)
            {
                case KeyCode.N:
                    if (evt.alt)
                    {
                        NextFrameWithCopy();
                    }
                    else if (evt.control)
                    {
                        LastFrame();
                    }
                    else
                    {
                        NextFrame();
                    }
                    evt.Use();
                    break;

                case KeyCode.S:
                    if (evt.control)
                    {
                        win.menuView.SaveConfig();
                    }
                    break;
            }
        }

        public int LoopIndex(int index, int min, int max)
        {
            return index > max ? (index - max - 1) : (index < min ? (max - index + 1) : index);
        }

        private void LastFrame()
        {
            int maxIndex = win.currentFrameCount - 1;
            if (maxIndex >= 0)
            {
                int index = win.frameSelectIndex - 1;
                index = LoopIndex(index, 0, maxIndex);
                win.frameSelectIndex = index;
            }
        }

        private void NextFrame()
        {
            int maxIndex = win.currentFrameCount - 1;
            if (maxIndex >= 0)
            {
                int index = win.frameSelectIndex + 1;
                index = LoopIndex(index, 0, maxIndex);
                win.frameSelectIndex = index;
            }
        }

        private void LastAttackRange()
        {
            int maxIndex = win.currentAttackRangeCount - 1;
            if (maxIndex >= 0)
            {
                int index = win.attackRangeSelectIndex - 1;
                index = LoopIndex(index, 0, maxIndex);
                win.attackRangeSelectIndex = index;
            }
        }

        private void NextAttackRange()
        {
            int maxIndex = win.currentAttackRangeCount - 1;
            if (maxIndex >= 0)
            {
                int index = win.attackRangeSelectIndex + 1;
                index = LoopIndex(index, 0, maxIndex);
                win.attackRangeSelectIndex = index;
            }
        }

        private void LastBodyRange()
        {
            int maxIndex = win.currentBodyRangeCount - 1;
            if (maxIndex >= 0)
            {
                int index = win.bodyRangeSelectIndex - 1;
                index = LoopIndex(index, 0, maxIndex);
                win.bodyRangeSelectIndex = index;
            }
        }

        private void NextBodyRange()
        {
            int maxIndex = win.currentBodyRangeCount - 1;
            if (maxIndex >= 0)
            {
                int index = win.bodyRangeSelectIndex + 1;
                index = LoopIndex(index, 0, maxIndex);
                win.bodyRangeSelectIndex = index;
            }
        }

        private void NextFrameWithCopy()
        {
            win.frameSelectIndex++;
            win.toolView.CopyPrevFrame();
        }

        private void OnWindowFunc(int id)
        {
            float buttonHeight = 40f;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("上一帧(Alt+N)", GUILayout.Height(buttonHeight)))
            {
                LastFrame();
            }
            if (GUILayout.Button("下一帧(N)", GUILayout.Height(buttonHeight)))
            {
                NextFrame();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("下一帧并拷贝上一帧(Ctrl+N)", GUILayout.Height(buttonHeight)))
            {
                NextFrameWithCopy();
            }

            string title = win.setting.enableAllControl ? "关闭" : "启用";
            if (GUILayout.Button($"{title}所有控制柄"))
            {
                win.setting.enableAllControl = !win.setting.enableAllControl;
            }

            if (!win.setting.enableAllControl)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("上一攻击框", GUILayout.Height(buttonHeight)))
                {
                    LastAttackRange();
                }
                if (GUILayout.Button("下一攻击框", GUILayout.Height(buttonHeight)))
                {
                    NextAttackRange();
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("上一身体框", GUILayout.Height(buttonHeight)))
                {
                    LastBodyRange();
                }
                if (GUILayout.Button("下一身体框", GUILayout.Height(buttonHeight)))
                {
                    NextBodyRange();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            FrameConfig frameConfig = win.currentFrame;
            if (frameConfig != null)
            {
                if (frameConfig.stayAttackRange)
                {
                    EditorGUILayout.HelpBox("攻击范围为保持状态，不可编辑", UnityEditor.MessageType.Warning);
                }
                if (frameConfig.stayBodyRange)
                {
                    EditorGUILayout.HelpBox("身体范围为保持状态，不可编辑", UnityEditor.MessageType.Warning);
                }
            }

            GUI.DragWindow();
        }
    }
}
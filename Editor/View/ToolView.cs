/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/1/10 11:02:09
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    public partial class ActionEditorSetting
    {
        public ToolView.Setting toolView = new ToolView.Setting();
    }

    /// <summary>
    /// ToolView
    /// </summary>
    [Serializable]
    public class ToolView : IView
    {
        [Serializable]
        public class Setting
        {
            public int frameCnt = 1;
            public bool copyAttackRanges;
            public bool copyBodyRanges;
            public int fromIndex;
            public int toIndex;
        }

        public Setting setting => win.setting.toolView;

        public override string title => "工具";

        public override bool useAre => true;

        private Vector2 scrollView = Vector2.zero;

        protected override void OnGUI(Rect rect)
        {
            StateConfig config = win.currentState;
            if (null == config)
            {
                return;
            }

            scrollView = EditorGUILayout.BeginScrollView(scrollView);

            string msg = "";

            msg += $"帧数:{win.currentFrames?.Count ?? 0}\n";

            AnimationClip clip = win.GetCurrentAnimationClip();
            if (clip != null)
            {
                float animFrameRate = 1 / clip.frameRate;
                float animLength = clip.length;
                int animFrameCnt = Mathf.CeilToInt(animLength / animFrameRate);

                msg += $"动画:\n\t帧率:{animFrameRate.ToString("0.###")}s\n\t长度:{animLength.ToString("0.###") }s\n\t帧数:{animFrameCnt}\n";
            }
            EditorGUILayout.HelpBox(msg, UnityEditor.MessageType.None);

            DrawFrameTool();

            EditorGUILayout.EndScrollView();
        }

        public override void OnUpdate()
        {
        }

        private void DrawFrameTool()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("帧操作");

            setting.frameCnt = EditorGUILayoutEx.DrawObject("帧数(N)", setting.frameCnt);

            if (GUILayout.Button("添加 N 帧到末尾"))
            {
                GUI.FocusControl(null);
                AddFrameToEnd();
            }

            if (GUILayout.Button("向前插入 N 帧"))
            {
                GUI.FocusControl(null);
                InsertFrameTo(true);
            }
            if (GUILayout.Button("向后插入 N 帧"))
            {
                GUI.FocusControl(null);
                InsertFrameTo(false);
            }

            if (GUILayout.Button("删除选择帧"))
            {
                GUI.FocusControl(null);
                if (win.frameSelectIndex < 0)
                {
                    EditorUtility.DisplayDialog("提示", "请选择一帧", "确定");
                    return;
                }

                if (win.frameSelectIndex >= 0)
                {
                    win.currentFrames.RemoveAt(win.frameSelectIndex);
                }
            }

            if (GUILayout.Button("从上一帧拷贝"))
            {
                CopyPrevFrame();
            }
            GUILayout.BeginHorizontal();
            setting.fromIndex = EditorGUILayoutEx.DrawObject("开始", setting.fromIndex);
            setting.toIndex = EditorGUILayoutEx.DrawObject("结束", setting.toIndex);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("粘贴范围"))
            {
                PasteRangeFrame();
            }
            if (GUILayout.Button("删除范围"))
            {
                DeleteRangeFrame();
            }

            if (GUILayout.Button("保持上一帧范围"))
            {
                for (int i = setting.fromIndex; i <= setting.toIndex; i++)
                {
                    FrameConfig config = win.currentFrames[i];

                    if(setting.copyAttackRanges)
                    {
                        config.attackRanges.Clear();
                        config.stayAttackRange = true;
                    }
                    
                    if(setting.copyBodyRanges)
                    {
                        config.bodyRanges.Clear();
                        config.stayBodyRange = true;
                    }
                }
            }
            GUILayout.Space(4f);

            GUILayout.Label("操作选项");

            win.setting.enableQuickKey = EditorGUILayoutEx.DrawObject("启用快捷键", win.setting.enableQuickKey);
            setting.copyAttackRanges = EditorGUILayoutEx.DrawObject("拷贝攻击范围", setting.copyAttackRanges);
            setting.copyBodyRanges = EditorGUILayoutEx.DrawObject("拷贝身体范围", setting.copyBodyRanges);

            win.setting.frameListViewRectHeight = EditorGUILayoutEx.DrawObject("帧视图高", win.setting.frameListViewRectHeight);
            win.setting.frameWidth = EditorGUILayoutEx.DrawObject("帧宽", win.setting.frameWidth);

            GUILayout.EndVertical();
        }

        private void DeleteRangeFrame()
        {
            if (win.frameSelectIndex < 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一帧", "确定");
                return;
            }

            int maxCount = win.currentFrames?.Count ?? 0;
            if (setting.fromIndex > setting.toIndex || setting.fromIndex < 0 || setting.toIndex < 0 || setting.fromIndex >= maxCount || setting.toIndex >= maxCount)
            {
                EditorUtility.DisplayDialog("提示", $"开始应小于结束，且都大于零，小于最大帧数 {maxCount} ", "确定");
                return;
            }

            win.currentState.frames.RemoveRange(setting.fromIndex, setting.toIndex - setting.fromIndex + 1);
        }

        public void CopyPrevFrame()
        {
            if (win.frameSelectIndex < 1)
            {
                EditorUtility.DisplayDialog("提示", "请选择有上一帧的一帧", "确定");
                return;
            }

            int prevIndex = win.frameSelectIndex - 1;
            FrameConfig prevFrame = win.currentFrames[prevIndex];
            List<RangeConfig> attackRanges = setting.copyAttackRanges ?  win.FindStayAttackRangeStartWith(prevIndex)  : null;
            List<RangeConfig> bodyRanges = setting.copyBodyRanges ? win.FindStayBodyRangeStartWith(prevIndex) : null;

            if (setting.copyAttackRanges)
            {
                win.currentFrame.CopyAttackRangeFrom(attackRanges);
            }

            if (setting.copyBodyRanges)
            {
                win.currentFrame.CopyBodyRangeFrom(bodyRanges);
            }
        }

        private void PasteRangeFrame()
        {
            if (win.frameSelectIndex < 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一帧", "确定");
                return;
            }

            int maxCount = win.currentFrames?.Count ?? 0;
            if (setting.fromIndex > setting.toIndex || setting.fromIndex < 0 || setting.toIndex < 0 || setting.fromIndex >= maxCount || setting.toIndex >= maxCount)
            {
                EditorUtility.DisplayDialog("提示", $"开始应小于结束，且都大于零，小于最大帧数 {maxCount} ", "确定");
                return;
            }

            FrameConfig frame = win.currentFrame;
            List<RangeConfig> attackRanges = setting.copyAttackRanges ? win.FindStayAttackRangeFromCurrent(true) : null;
            List<RangeConfig> bodyRanges = setting.copyBodyRanges ? win.FindStayBodyRangeFromCurrent(true) : null;

            for (int i = setting.fromIndex; i <= setting.toIndex; i++)
            {
                if (i == win.frameSelectIndex)
                {//自身不用拷贝，跳过
                    continue;
                }

                FrameConfig config = win.currentFrames[i];
                if (setting.copyAttackRanges)
                {
                    config.CopyAttackRangeFrom(attackRanges);
                }
                if (setting.copyBodyRanges)
                {
                    config.CopyBodyRangeFrom(bodyRanges);
                }
            }
        }

        private void InsertFrameTo(bool forward)
        {
            if (win.frameSelectIndex < 0)
            {
                EditorUtility.DisplayDialog("提示", "请选择一帧", "确定");
                return;
            }

            FrameConfig frame = win.currentFrame;
            List<RangeConfig> attackRanges = setting.copyAttackRanges ? win.FindStayAttackRangeFromCurrent() : null;
            List<RangeConfig> bodyRanges = setting.copyBodyRanges ? win.FindStayBodyRangeFromCurrent() : null;

            if (forward || win.frameSelectIndex < win.currentFrames.Count - 1)
            {
                int insertIndex = forward ? win.frameSelectIndex : win.frameSelectIndex + 1;
                for (int i = 0; i < setting.frameCnt; i++)
                {
                    win.currentFrames.Insert(insertIndex, new FrameConfig(attackRanges, bodyRanges));
                }

                if (forward)
                {
                    win.frameSelectIndex += setting.frameCnt;
                }
            }
            else
            {
                AddFrameToEnd();
            }
        }

        private void AddFrameToEnd()
        {
            List<RangeConfig> attackRanges = null;
            List<RangeConfig> bodyRanges = null;

            if (win.frameSelectIndex >= 0)
            {
                FrameConfig frame = win.currentFrame;
                attackRanges = setting.copyAttackRanges ? win.FindStayAttackRangeFromCurrent() : null;
                bodyRanges = setting.copyBodyRanges ? win.FindStayBodyRangeFromCurrent() : null;
            }

            for (int i = 0; i < setting.frameCnt; i++)
            {
                win.currentFrames.Add(new FrameConfig(attackRanges, bodyRanges));
            }
        }
    }
}
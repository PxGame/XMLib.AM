/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:37:58
 */

using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// FrameListView
    /// </summary>
    [Serializable]
    public class FrameListView : IView
    {
        public override string title => $"帧序列({win.setting.frameRate}s)";
        public override bool useAre => false;

        private Vector2 actionViewScroll = Vector2.zero;

        private bool playFrame = false;

        private float playTimer = 0f;
        private float playSpeed = 1f;

        private float toolHeight = 22f;

        protected override void OnGUI(Rect rect)
        {
            List<FrameConfig> frames = win.currentFrames;
            List<object> actions = win.currentActions;

            if (null == frames || null == actions || frames.Count == 0)
            {
                EditorGUI.LabelField(rect, "没有帧可编辑");
                return;
            }

            Rect frameRt = new Rect(rect.x, rect.y, rect.width, rect.height - toolHeight);
            Rect toolRt = new Rect(rect.x, rect.y + frameRt.height, rect.width, toolHeight);

            DrawFrames(frames, actions, frameRt);
            DrawTool(frames, toolRt);
        }

        private void DrawFrames(List<FrameConfig> frames, List<object> actions, Rect rect)
        {
            int frameCnt = frames.Count;

            float headWidth = 20f;
            float topHeight = 30f;

            float frameWidth = win.setting.frameWidth;
            float frameSpace = 4f;

            float actionHeight = 30f;
            float actionSpace = 4f;
            float actionOffset = 0f;

            float barSize = 16f;

            float viewHeight = rect.height - barSize - topHeight;
            float viewWidth = frameWidth * frameCnt;

            float minViewWidth = rect.width - barSize - headWidth;
            if (viewWidth < minViewWidth)
            {
                viewWidth = minViewWidth;
            }

            float actionsHeight = actionHeight * actions.Count;
            if (actionsHeight > viewHeight)
            {
                viewHeight = actionsHeight + actionHeight;
            }

            Rect framePosition = new Rect(rect.x + headWidth, rect.y, rect.width - headWidth - barSize, rect.height - barSize);
            Rect frameView = new Rect(framePosition.x, framePosition.y, viewWidth, rect.height - barSize);

            Rect actionIdPosition = new Rect(rect.x, rect.y + topHeight, headWidth, rect.height - barSize - topHeight);
            Rect actionIdView = new Rect(actionIdPosition.x, actionIdPosition.y, headWidth, viewHeight);

            Rect actionPosition = new Rect(rect.x + headWidth, rect.y + topHeight, rect.width - headWidth, rect.height - topHeight);
            Rect actionView = new Rect(actionPosition.x, actionPosition.y, viewWidth, viewHeight);

            Rect beginFrameRt = new Rect(frameView.x, frameView.y, frameWidth, frameView.height);
            Rect beginActionIdRect = new Rect(actionIdView.x, actionIdView.y, headWidth, actionHeight);

            Rect beginActionRt = new Rect(actionView.x, actionView.y, actionView.width, actionHeight);
            Rect beginActionBgRt = new Rect(actionView.x, actionView.y, actionView.width, actionHeight);

            Rect beginFrameBtnRt = beginFrameRt;
            beginFrameBtnRt.height = topHeight;
            Rect beginFrameBgRt = beginFrameRt;
            beginFrameBgRt.y += topHeight;
            beginFrameBgRt.height = frameView.height - topHeight;

            //-----------------------------------------------------

            #region 帧

            GUI.BeginScrollView(framePosition, Vector2.right * actionViewScroll.x, frameView, GUIStyle.none, GUIStyle.none);
            for (int i = 0; i < frameCnt; i++)
            {
                Rect btnRt = beginFrameBtnRt;
                btnRt.x += frameWidth * i;
                btnRt.width -= frameSpace;

                Rect bgRt = beginFrameBgRt;
                bgRt.x += frameWidth * i;
                bgRt.width -= frameSpace;

                bool selected = win.frameSelectIndex == i;

                FrameConfig config = win.currentFrames[i];

                string title = string.Format("{0}\n{1}|{2}",
                 i, 
                 config.stayAttackRange ? "←" : (config.attackRanges?.Count ?? 0).ToString(),
                 config.stayBodyRange ? "←" : (config.bodyRanges?.Count ?? 0).ToString());
                if (GUI.Button(btnRt, title, selected ? AEStyles.item_head_select : AEStyles.item_head_normal))
                {
                    win.frameSelectIndex = selected ? -1 : i;
                }
                GUI.Box(bgRt, GUIContent.none, selected ? AEStyles.item_body_select : AEStyles.item_body_normal);
            }
            GUI.EndScrollView();

            #endregion 帧

            //-----------------------------------------------------

            #region 控制条

            actionViewScroll = GUI.BeginScrollView(actionPosition, actionViewScroll, actionView, true, true);
            for (int i = 0; i < actions.Count; i++)
            {
                object action = actions[i];

                int beginFrame = 0;
                int endFrame = frameCnt - 1;

                IHoldFrames holdFrames = action as IHoldFrames;
                if (holdFrames != null)
                {
                    beginFrame = holdFrames.GetBeginFrame();
                    endFrame = holdFrames.GetEndFrame();
                }

                //minmaxslider
                Rect rt = beginActionRt;
                rt.y += actionHeight * i + actionOffset;
                rt.height -= (actionSpace + actionOffset);

                bool selected = win.actionSelectIndex == i;

                float beginValue = beginFrame * frameWidth;
                float endValue = (endFrame + 1) * frameWidth - 2 * EditorGUIEx.minMaxThumbWidth - frameSpace;

                bool clicked = EditorGUIEx.MinMaxSlider(rt, ref beginValue, ref endValue, 0, viewWidth - 2 * EditorGUIEx.minMaxThumbWidth);
                if (clicked)
                {
                    win.actionSelectIndex = selected ? -1 : i;
                }

                if (holdFrames != null)
                {
                    if (holdFrames.EnableBeginEnd())
                    {
                        //校验
                        beginFrame = Mathf.RoundToInt(beginValue / frameWidth);
                        endFrame = Mathf.RoundToInt((endValue + 2 * EditorGUIEx.minMaxThumbWidth + frameSpace) / frameWidth - 1);
                        beginFrame = Mathf.Clamp(beginFrame, 0, frameCnt - 1);
                        endFrame = Mathf.Clamp(endFrame, 0, frameCnt - 1);
                        if (endFrame < beginFrame)
                        {
                            endFrame = beginFrame;
                        }
                        //
                    }
                    else
                    {//设置最大值
                        beginFrame = 0;
                        endFrame = frameCnt - 1;
                    }

                    holdFrames.SetBeginFrame(beginFrame);
                    holdFrames.SetEndFrame(endFrame);
                }
            }
            GUI.EndScrollView();

            #endregion 控制条

            //-----------------------------------------------------

            #region 动作序号

            GUI.BeginScrollView(actionIdPosition, Vector2.up * actionViewScroll.y, actionIdView, GUIStyle.none, GUIStyle.none);
            for (int i = 0; i < actions.Count; i++)
            {
                Rect rt = beginActionIdRect;
                rt.y += actionHeight * i;
                rt.height -= actionSpace;

                bool selected = win.actionSelectIndex == i;

                if (GUI.Button(rt, $"{i}", selected ? AEStyles.item_head_select : AEStyles.item_head_normal))
                {
                    win.actionSelectIndex = selected ? -1 : i;
                }
            }
            GUI.EndScrollView();

            #endregion 动作序号

            //-------------------------------------------

            #region 动作信息

            GUI.BeginScrollView(actionPosition, actionViewScroll, actionView, true, true);
            for (int i = 0; i < actions.Count; i++)
            {
                object action = actions[i];

                //label
                Rect labelRt = beginActionBgRt;
                labelRt.y += actionHeight * i;
                labelRt.height -= actionSpace;
                labelRt.width = viewWidth;

                bool selected = win.actionSelectIndex == i;

                if (GUI.Button(labelRt, $"{action.ToString()}", AEStyles.frame_label))
                {
                    win.actionSelectIndex = selected ? -1 : i;
                }
            }
            GUI.EndScrollView();

            #endregion 动作信息
        }

        private void DrawTool(List<FrameConfig> configs, Rect rect)
        {
            int maxFrames = configs.Count;

            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal(AEStyles.list_tool_bg, GUILayout.Height(18f), GUILayout.ExpandHeight(false));

            if (GUILayout.Button("上一帧", GUILayout.Width(65)))
            {
                int index = win.frameSelectIndex - 1;
                win.frameSelectIndex = Mathf.Clamp(index, -1, maxFrames - 1);
            }
            if (GUILayout.Button("下一帧", GUILayout.Width(65)))
            {
                int index = win.frameSelectIndex + 1;
                win.frameSelectIndex = Mathf.Clamp(index, -1, maxFrames - 1);
            }
            if (GUILayout.Button(playFrame ? "停止" : "播放"))
            {
                playTimer = 0f;
                playFrame = !playFrame;
            }

            GUILayout.Space(5);
            playSpeed = EditorGUILayout.Slider(playSpeed, 0f, 1f, GUILayout.MaxWidth(150));
            GUILayout.Space(10);
            win.frameSelectIndex = EditorGUILayout.IntSlider(win.frameSelectIndex, -1, maxFrames - 1, GUILayout.MaxWidth(300f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public override void OnUpdate()
        {
            if (!playFrame)
            {
                return;
            }
            int maxIndex = win.currentFrames?.Count ?? -1;
            if (maxIndex < 0)
            {
                return;
            }

            playTimer += Time.deltaTime * playSpeed;

            int index = win.frameSelectIndex;

            while (playTimer > win.setting.frameRate)
            {
                playTimer -= win.setting.frameRate;
                index += 1;

                if (index >= maxIndex)
                {
                    index = 0;
                }
            }
            win.frameSelectIndex = index;

            win.Repaint();
        }
    }
}
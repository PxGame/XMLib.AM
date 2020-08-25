/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:37:28
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// StateSetView
    /// </summary>
    public class StateSetView : IDataView
    {
        public ActionEditorWindow win { get; set; }

        public string title => "状态设置";
        public bool useAre => true;

        private Vector2 scrollView = Vector2.zero;

        public void OnGUI(Rect rect)
        {
            StateConfig config = win.currentState;
            if (null == config)
            {
                return;
            }

            scrollView = EditorGUILayout.BeginScrollView(scrollView);

            EditorGUI.BeginChangeCheck();

            config.stateName = EditorGUILayoutEx.DrawObject("状态名", config.stateName);
            config.animName = EditorGUILayoutEx.DrawObject("动画名", config.animName);
            config.fadeTime = EditorGUILayoutEx.DrawObject("过度时间", config.fadeTime);

            config.enableLoop = EditorGUILayoutEx.DrawObject("循环", config.enableLoop);
            if (!config.enableLoop)
            {
                config.nextStateName = EditorGUILayoutEx.DrawObject("下一个状态", config.nextStateName);
            }

            if (EditorGUI.EndChangeCheck())
            {
                win.actionMachineDirty = true;
            }

            EditorGUILayout.EndScrollView();
        }

        public void OnUpdate()
        {
        }

        public object CopyData()
        {
            return win.currentState;
        }

        public void PasteData(object data)
        {
            if (win.currentState != null && data is StateConfig state)
            {
                win.currentStates[win.stateSelectIndex] = state;
            }
        }
    }
}
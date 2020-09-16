/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:36:42
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// StateListView
    /// </summary>
    public class StateListView : IDataView
    {
        public ActionEditorWindow win { get; set; }

        public string title => "状态列表";
        public bool useAre => true;

        public Vector2 scrollPos;

        public void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical(AEStyles.box);
            win.config.firstStateName = EditorGUILayoutEx.DrawObject("起始状态名", win.config.firstStateName);
            GUILayout.EndVertical();
            GUILayout.Space(4);
            win.stateSelectIndex = EditorGUILayoutEx.DrawList(win.config.states, win.stateSelectIndex, ref scrollPos, NewState, ActionEditorUtility.StateDrawer);
        }

        private void NewState(Action<StateConfig> adder)
        {
            adder(new StateConfig());
        }

        public object CopyData()
        {
            return win.currentState;
        }

        public void PasteData(object data)
        {
            if (win.currentStates != null && data is StateConfig configs)
            {
                win.currentStates.Add(configs);
            }
        }

        public void OnUpdate()
        {
        }
    }
}
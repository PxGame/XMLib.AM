/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:36:42
 */

using System;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// StateListView
    /// </summary>
    [Serializable]
    public class StateListView : IDataView
    {
        public override string title => "状态列表";
        public override bool useAre => true;

        private Vector2 scrollPos;

        protected override void OnGUI(Rect rect)
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

        public override object CopyData()
        {
            return win.currentState;
        }

        public override void PasteData(object data)
        {
            if (win.currentStates != null && data is StateConfig configs)
            {
                win.currentStates.Add(configs);
            }
        }

        public override void OnUpdate()
        {
        }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/12/24 0:08:57
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace XMLib.AM
{
    /// <summary>
    /// TestActionMachineEditor
    /// </summary>
    [CustomEditor(typeof(ActionMachineTest))]
    public class ActionMachineTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ActionMachineTest actionMachine = (ActionMachineTest)target;

            base.OnInspectorGUI();

            if (GUILayout.Button("打开编辑器"))
            {
                ActionEditorWindow.ShowEditor(actionMachine.gameObject, actionMachine.config);
            }
        }
    }
}
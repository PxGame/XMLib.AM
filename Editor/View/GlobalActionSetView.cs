/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:40:42
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// GlobalActionSetView
    /// </summary>
    [Serializable]
    public class GlobalActionSetView<ControllerType, FloatType> : IView<ControllerType, FloatType> where FloatType : struct
    {
        public ActionEditorWindow<ControllerType, FloatType> win { get; set; }

        public string title => "全局动作设置";
        public bool useAre => true;

        private Vector2 scrollView = Vector2.zero;

        public void OnGUI(Rect rect)
        {
            object obj = win.currentGlobalAction;
            if (null == obj)
            {
                return;
            }

            scrollView = EditorGUILayout.BeginScrollView(scrollView);
            EditorGUILayoutEx.DrawObject(GUIContent.none, obj, obj.GetType());
            EditorGUILayout.EndScrollView();
        }

        public void OnUpdate()
        {
        }
    }
}
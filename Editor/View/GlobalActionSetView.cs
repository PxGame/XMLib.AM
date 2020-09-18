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
    public class GlobalActionSetView : IView
    {
        public ActionEditorWindow win { get; set; }

        public string title => "全局动作设置";
        public bool useAre => true;

        public void OnGUI(Rect rect)
        {
            object obj = win.currentGlobalAction;
            if (null == obj)
            {
                return;
            }

            Type type = obj.GetType();

            FieldInfo[] fieldInfos = type.GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                DrawField(obj, fieldInfo);
            }
        }

        private void DrawField(object obj, FieldInfo fieldInfo)
        {
            EditorGUILayoutEx.DrawField(obj, fieldInfo);
        }

        public void OnUpdate()
        {
        }
    }
}
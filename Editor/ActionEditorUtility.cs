/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/30 11:07:10
 */

using System;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// ActionEditorUtility
    /// </summary>
    public static class ActionEditorUtility
    {
        public static bool HasOpenInstances(Type windowType)
        {
            UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(windowType);
            return array != null && array.Length != 0;
        }

        public static void ItemDrawer<T>(int index, ref bool selected, T obj)
        {
            if (GUILayout.Button($"{index}", selected ? AEStyles.item_head_select : AEStyles.item_head_normal, GUILayout.ExpandHeight(true), GUILayout.Width(15)))
            {
                GUI.FocusControl(null);
                selected = !selected;
            }
            if (GUILayout.Button($"{obj?.GetType().GetSimpleName()}", selected ? AEStyles.item_body_select : AEStyles.item_body_normal, GUILayout.Height(30f), GUILayout.ExpandWidth(true)))
            {
                GUI.FocusControl(null);
                selected = !selected;
            }
        }

        public static void StateDrawer<T>(int index, ref bool selected, T obj) where T : StateConfig
        {
            if (GUILayout.Button($"{index}", selected ? AEStyles.item_head_select : AEStyles.item_head_normal, GUILayout.ExpandHeight(true), GUILayout.Width(15)))
            {
                GUI.FocusControl(null);
                selected = !selected;
            }
            if (GUILayout.Button($"{obj?.stateName}", selected ? AEStyles.item_body_select : AEStyles.item_body_normal, GUILayout.Height(30f), GUILayout.ExpandWidth(true)))
            {
                GUI.FocusControl(null);
                selected = !selected;
            }
        }

        public static void RangeConfigDrawer(int index, ref bool selected, RangeConfig obj)
        {
            if (GUILayout.Button($"{index}", selected ? AEStyles.item_head_select : AEStyles.item_head_normal, GUILayout.ExpandHeight(true), GUILayout.Width(15)))
            {
                GUI.FocusControl(null);
                selected = !selected;
            }

            EditorGUILayoutEx.DrawObject(GUIContent.none, obj);
        }
    }
}
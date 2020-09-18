/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:38:45
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// AttackRangeListView
    /// </summary>
    [Serializable]
    public class AttackRangeListView : IDataView
    {
        public ActionEditorWindow win { get; set; }

        public string title => "攻击范围";

        public bool useAre => true;
        private Vector2 scrollPos;

        public void OnGUI(Rect rect)
        {
            List<RangeConfig> configs = win.currentAttackRanges;
            if (null == configs)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            win.attackRangeSelectIndex = EditorGUILayoutEx.DrawList(configs, win.attackRangeSelectIndex, ref scrollPos, NewRange, ActionEditorUtility.RangeConfigDrawer);
            if (EditorGUI.EndChangeCheck())
            {
                //win.configModification = true;
            }
        }

        private void NewRange(Action<RangeConfig> adder)
        {
            adder(new RangeConfig());
        }

        public object CopyData()
        {
            return win.currentAttackRanges;
        }

        public void PasteData(object data)
        {
            if (win.currentAttackRanges != null && data is List<RangeConfig> ranges)
            {
                win.currentAttackRanges.Clear();
                win.currentAttackRanges.AddRange(ranges);
            }
        }

        public void OnUpdate()
        {
        }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:28:13
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    public abstract class IView
    {
        public abstract string title { get; }
        public abstract bool useAre { get; }
        public abstract void OnGUI(Rect rect);
        public abstract void OnUpdate();

        public EditorWindow popWindow { get; protected set; }

        public bool isPop => popWindow != null;

        public void ShowPopWindow()
        {
            popWindow = ViewWindow.Show(this);
        }

        public void HidePopWindow()
        {
            if (popWindow == null)
            {
                return;
            }

            popWindow.Close();
            popWindow = null;
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
            HidePopWindow();
        }

        public virtual void OnRepaint()
        {
            if (popWindow != null)
            {
                popWindow.Repaint();
            }
        }

        public void OnPopDestroy()
        {
            popWindow = null;
        }
    }

    /// <summary>
    /// IView
    /// </summary>
    public abstract class IView<ControllerType, FloatType> : IView where FloatType : struct
    {
        public ActionEditorWindow<ControllerType, FloatType> win { get; set; }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:32:18
 */

using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// IDataView
    /// </summary>
    public abstract class IDataView : IView
    {
        public abstract object CopyData();

        public abstract void PasteData(object data);

        protected override void OnHeaderDraw()
        {
            base.OnHeaderDraw();

            if (GUILayout.Button("C", AEStyles.view_head, GUILayout.Width(20)))
            {
                GUI.FocusControl(null);
                win.copyBuffer = CopyData();
            }

            if (GUILayout.Button("P", AEStyles.view_head, GUILayout.Width(20)))
            {
                GUI.FocusControl(null);
                object data = win.copyBuffer;
                if (data != null)
                {
                    PasteData(data);
                }
            }
        }
    }
}
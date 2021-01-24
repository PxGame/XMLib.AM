/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:28:13
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// IView
    /// </summary>
    public interface IView<ControllerType, FloatType> where FloatType : struct
    {
        ActionEditorWindow<ControllerType, FloatType> win { get; set; }
        string title { get; }
        bool useAre { get; }

        void OnGUI(Rect rect);

        void OnUpdate();
    }
}
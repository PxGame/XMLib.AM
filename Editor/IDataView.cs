/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:32:18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// IDataView
    /// </summary>
    public abstract class IDataView<ControllerType, FloatType> : IView<ControllerType, FloatType> where FloatType : struct
    {
        public abstract object CopyData();

        public abstract void PasteData(object data);
    }
}
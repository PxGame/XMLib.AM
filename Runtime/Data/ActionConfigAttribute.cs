/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/4 9:59:33
 */

using System;

namespace XMLib.AM
{
    /// <summary>
    /// ActionConfigAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ActionConfigAttribute : Attribute
    {
        public Type handlerType { get; protected set; }

        public ActionConfigAttribute(Type handlerType)
        {
            this.handlerType = handlerType;
        }
    }
}
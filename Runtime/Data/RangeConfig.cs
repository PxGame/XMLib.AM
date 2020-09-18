/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 14:16:49
 */

using System;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// RangeConfig
    /// </summary>
    [Serializable]
    public class RangeConfig
    {
        #region data

        public interface IItem
        {
            IItem Clone();
        }

        [Serializable]
        public class RectItem : IItem
        {
            public Vector2 offset = Vector2.up;
            public Vector2 size = Vector2.one;

            public IItem Clone()
            {
                return new RectItem()
                {
                    offset = this.offset,
                    size = this.size,
                };
            }
        }

        [Serializable]
        public class CircleItem : IItem
        {
            public Vector2 offset = Vector2.up;
            public float radius = 1f;

            public IItem Clone()
            {
                return new CircleItem()
                {
                    offset = this.offset,
                    radius = this.radius
                };
            }
        }

        [Serializable]
        public class BoxItem : IItem
        {
            public Vector3 offset = Vector3.up;
            public Vector3 size = Vector3.one;

            public IItem Clone()
            {
                return new BoxItem()
                {
                    offset = this.offset,
                    size = this.size
                };
            }
        }

        [Serializable]
        public class SphereItem : IItem
        {
            public Vector3 offset = Vector3.up;
            public float radius = 1f;

            public IItem Clone()
            {
                return new SphereItem()
                {
                    offset = this.offset,
                    radius = this.radius
                };
            }
        }

        #endregion data

        [ObjectTypes(typeof(RectItem), typeof(CircleItem), typeof(BoxItem), typeof(SphereItem))]
        
        [SerializeReference]
        public IItem value;

        public Type GetValueType() => value?.GetType() ?? null;

        public RangeConfig()
        {
        }

        public RangeConfig(RangeConfig config)
        {
            this.value = config.value.Clone();
        }
    }
}
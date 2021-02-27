/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 14:16:49
 */

using System;
using UnityEngine;


#if USE_FIXPOINT
using Single = FPPhysics.Fix64;
using Vector2 = FPPhysics.Vector2;
using Vector3 = FPPhysics.Vector3;
using Quaternion = FPPhysics.Quaternion;
using Matrix4x4 = FPPhysics.Matrix4x4;
using Mathf = FPPhysics.FPUtility;
using ControllerType = System.Object;
#else
using Single = System.Single;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Mathf = UnityEngine.Mathf;
using ControllerType = System.Object;
#endif

namespace XMLib.AM
{
    /// <summary>
    /// RangeConfig
    /// </summary>
    [Serializable]
    public class RangeConfig
    {
        [Ranges.RangeTypes]
        [SerializeReference]
        public Ranges.IItem value;

        public Type GetValueType() => value?.GetType() ?? null;

        public RangeConfig()
        {
        }

        public RangeConfig(RangeConfig config)
        {
            this.value = config.value.Clone();
        }
    }

    namespace Ranges
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
            public Single radius = 1;

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
            public Single radius = 1;

            public IItem Clone()
            {
                return new SphereItem()
                {
                    offset = this.offset,
                    radius = this.radius
                };
            }
        }

        public class RangeTypesAttribute : ObjectTypesAttribute
        {
            public override Type baseType => typeof(IItem);
        }

        #endregion data
    }
}
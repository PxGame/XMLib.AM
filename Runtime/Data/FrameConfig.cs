/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 14:15:04
 */

using System.Collections.Generic;

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
    /// FrameConfig
    /// </summary>
    [System.Serializable]
    public class FrameConfig
    {
        public bool stayAttackRange;
        public bool stayBodyRange;
        public List<RangeConfig> attackRanges = new List<RangeConfig>();
        public List<RangeConfig> bodyRanges = new List<RangeConfig>();

        public FrameConfig()
        {
        }

        public FrameConfig(List<RangeConfig> attackRanges, List<RangeConfig> bodyRanges)
        {
            CopyAttackRangeFrom(attackRanges);
            CopyBodyRangeFrom(bodyRanges);
        }

        public List<RangeConfig> CopyAttackRanges()
        {
            return CopyRanges(attackRanges);
        }

        public List<RangeConfig> CopyBodyRanges()
        {
            return CopyRanges(bodyRanges);
        }

        private List<RangeConfig> CopyRanges(List<RangeConfig> ranges)
        {
            List<RangeConfig> copy = new List<RangeConfig>(ranges.Count);
            foreach (var item in ranges)
            {
                copy.Add(new RangeConfig(item));
            }
            return copy;
        }

        public void CopyAttackRangeFrom(List<RangeConfig> ranges)
        {
            attackRanges.Clear();

            if (ranges == null)
            {
                return;
            }

            foreach (var item in ranges)
            {
                attackRanges.Add(new RangeConfig(item));
            }
            stayAttackRange = false;
        }

        public void CopyBodyRangeFrom(List<RangeConfig> ranges)
        {
            bodyRanges.Clear();

            if (ranges == null)
            {
                return;
            }

            foreach (var item in ranges)
            {
                bodyRanges.Add(new RangeConfig(item));
            }
            stayBodyRange = false;
        }
    }
}
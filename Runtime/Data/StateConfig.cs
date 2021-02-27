/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 14:14:41
 */

using System.Collections.Generic;
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
    /// StateConfig
    /// </summary>
    [System.Serializable]
    public class StateConfig
    {
        public string stateName = "State";
        public List<string> animNames;
        public int dafualtAnimIndex = 0;
        public Single fadeTime = 1 / (Single)20;
        public bool enableLoop = false;
        public string nextStateName = "";
        public int nextAnimIndex = -1;

        public List<FrameConfig> frames = new List<FrameConfig>();

        [SerializeReference]
        public List<object> actions = new List<object>();

        public override string ToString() => stateName;

        public string defaultAnimaName => GetAnimName(dafualtAnimIndex);

        public string GetAnimName(int index)
        {
            return animNames?.Count > index ? animNames[index] : string.Empty;
        }

        public FrameConfig GetBodyRangesFrame(int frameIndex)
        {
            if (frames.Count == 0 || frameIndex < 0)
            {
                return null;
            }

            frameIndex %= frames.Count;
            FrameConfig config = frames[frameIndex];

            while (config.stayBodyRange)
            {
                --frameIndex;
                if (frameIndex < 0)
                {
                    return null;
                }
                config = frames[frameIndex];
            }

            return config;
        }

        public List<RangeConfig> GetBodyRanges(int frameIndex)
        {
            return GetBodyRangesFrame(frameIndex)?.bodyRanges;
        }

        public FrameConfig GetAttackRangesFrame(int frameIndex)
        {
            if (frames.Count == 0 || frameIndex < 0)
            {
                return null;
            }

            frameIndex %= frames.Count;
            FrameConfig config = frames[frameIndex];

            while (config.stayAttackRange)
            {
                --frameIndex;
                if (frameIndex < 0)
                {
                    return null;
                }
                config = frames[frameIndex];
            }

            return config;
        }

        public List<RangeConfig> GetAttackRanges(int frameIndex)
        {
            return GetAttackRangesFrame(frameIndex)?.attackRanges;
        }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 14:14:41
 */

using System.Collections.Generic;
using UnityEngine;

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
        public float fadeTime = 0.05f;
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

        public List<RangeConfig> GetBodyRanges(int frameIndex)
        {
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

            return config.bodyRanges;
        }
    }
}
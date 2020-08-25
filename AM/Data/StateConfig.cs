/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 14:14:41
 */

using System.Collections.Generic;

namespace XMLib.AM
{
    /// <summary>
    /// StateConfig
    /// </summary>
    [System.Serializable]
    public class StateConfig
    {
        public string stateName = "State";
        public string animName = "Animation";
        public float fadeTime = 0.05f;
        public bool enableLoop = false;
        public string nextStateName = "";

        public List<FrameConfig> frames = new List<FrameConfig>();
        public List<object> actions = new List<object>();

        public override string ToString() => stateName;
    }
}
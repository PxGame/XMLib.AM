/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/21 11:39:30
 */

using System;

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
    /// IHoldFrames
    /// </summary>
    public interface IHoldFrames
    {
        int GetBeginFrame();

        void SetBeginFrame(int frameIndex);

        int GetEndFrame();

        void SetEndFrame(int frameIndex);

        bool EnableBeginEnd();

        bool EnableLoop();
    }

    [Serializable]
    public abstract class HoldFrames : IHoldFrames
    {
        #region IHoldFrames

        [EnableToggle()]
        public bool enableBeginEnd = true;

        [EnableToggleItem(nameof(enableBeginEnd))]
        public int beginFrame;

        [EnableToggleItem(nameof(enableBeginEnd))]
        public int endFrame;

        /// <summary>
        /// enableBeginEnd 为 true 时，才有效
        /// </summary>
        [EnableToggleItem(nameof(enableBeginEnd))]
        public bool enableLoop = true;

        public bool EnableLoop() => enableLoop;

        public bool EnableBeginEnd() => enableBeginEnd;

        public int GetBeginFrame() => beginFrame;

        public void SetBeginFrame(int frameIndex) => beginFrame = frameIndex;

        public int GetEndFrame() => endFrame;

        public void SetEndFrame(int frameIndex) => endFrame = frameIndex;

        #endregion IHoldFrames
    }
}
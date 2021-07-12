/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/31 10:04:20
 */

using System;
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

namespace XMLib.AM.Deprecated
{
    [Flags]
    public enum ActionMachineEvent
    {
        None = 0b0000_0000,
        FrameChanged = 0b0000_0001,
        StateChanged = 0b0000_0010,
        AnimChanged = 0b0000_0100,
        HoldAnimDuration = 0b0000_1000,

        All = 0b1111_1111
    }

    public interface IActionMachine
    {
        bool isDebug { get; set; }
        ControllerType controller { get; }
        Single animStartTime { get; }

        void Initialize(string config, ControllerType controller);

        void LogicUpdate(Single delta);

        void ChangeState(string stateName, int priority = 0, int animIndex = -1, Single animStartTime = default);

        ActionMachineEvent eventTypes { get; }

        int animIndex { get; }

        int frameIndex { get; }
        int stateBeginFrameIndex { get; }

        int waitFrameCnt { get; set; }
        int waitFrameDelay { get; set; }

        string configName { get; }
        string stateName { get; }

        string GetAnimName();

        MachineConfig GetMachineConfig();

        StateConfig GetStateConfig();

        int GetStateFrameIndex();

        FrameConfig GetStateFrame();

        int GetStateLoopCnt();

        List<RangeConfig> GetAttackRanges();

        List<RangeConfig> GetBodyRanges();

        void Destroy();

        void ChangeAnim(int animIndex, bool holdDuration = false);

        void ReplayAnim();
    }
}
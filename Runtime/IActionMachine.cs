/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/31 10:04:20
 */

using System;

namespace XMLib.AM
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

    /// <summary>
    /// IActionMachine<IActionController, Single>
    /// </summary>
    public interface IActionMachine<ControllerType, FloatType> : IActionMachine where FloatType : struct
    {
        ControllerType controller { get; }
        FloatType animStartTime { get; }

        void Initialize(string config, ControllerType controller);
        void LogicUpdate(FloatType delta);
        void ChangeState(string stateName, int priority = 0, int animIndex = -1, FloatType animStartTime = default);
    }

    public interface IActionMachine
    {

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


        void Destroy();

        void ChangeAnim(int animIndex, bool holdDuration = false);

        void ReplayAnim();
    }
}
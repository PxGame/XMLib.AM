/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/31 10:04:20
 */

using System;

namespace XMLib.AM
{
    /// <summary>
    /// IActionMachine
    /// </summary>
    public interface IActionMachine
    {
        object controller { get; }

        IInputContainer input { get; }

        bool isFrameChanged { get; }
        bool isStateChanged { get; }
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

        void Initialize(string config, object controller, IInputContainer input);

        void Destroy();

        void LogicUpdate(float delta);

        void ChangeState(string stateName, int priority = 0, int animIndex = -1);

        #region data operations

        object this[int tag] { get; set; }

        void SetData(int tag, object data);

        bool RemoveData(int tag);

        bool GetData<T>(int tag, out T data);

        bool GetData(int tag, Type type, out object data);

        T GetData<T>(int tag);

        T GetData<T>(int tag, T defaultValue);

        void ClearData();

        #endregion data operations
    }
}
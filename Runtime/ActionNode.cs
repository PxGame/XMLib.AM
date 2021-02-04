/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/5 17:51:35
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
    /// ActionNode
    /// </summary>
    public class ActionNode
    {
        public IActionMachine actionMachine;
        public int beginFrameIndex;
        public object config;
        public IActionHandler handler;
        public object data;
        public bool isUpdating { get; private set; } = false;
        public int updateCnt { get; private set; } = 0;

        public override string ToString()
        {
            return $"动作节点：{actionMachine.configName}-{actionMachine.GetStateConfig().stateName}-{config.GetType().Name}-{actionMachine.GetStateFrameIndex()}";
        }

        public void Reset()
        {
            actionMachine = null;
            beginFrameIndex = -1;
            config = null;
            handler = null;
            isUpdating = false;
            updateCnt = 0;
            data = null;
        }

        public void InvokeEnter()
        {
            updateCnt = 0;
            isUpdating = true;
            handler.Enter(this);
        }

        public void InvokeExit()
        {
            handler.Exit(this);
            isUpdating = false;
        }

        public void InvokeUpdate(Single deltaTime)
        {
            if (!isUpdating)
            {
                return;
            }

            handler.Update(this, deltaTime);
            updateCnt++;
        }
    }
}
/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/29 14:14:00
 */

using System;
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
    /// MachineConfig
    /// </summary>
    [Serializable]
    public class MachineConfig
    {
        public string firstStateName;
        public List<StateConfig> states = new List<StateConfig>();

        [SerializeReference]
        public List<object> globalActions = new List<object>();
    }
}
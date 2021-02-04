/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/2/15 11:45:21
 */

using System;
using System.Collections.Generic;
using System.Reflection;


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
    /// ActionMachineHelper
    /// </summary>
    public class ActionMachineHelper
    {
        #region static

        public static Func<string, MachineConfig> loader { get; private set; }

        public static Dictionary<string, MachineConfig> loadedConfig => machineConfigDict;

        private static Dictionary<Type, IActionHandler> actionHandlerDict = new Dictionary<Type, IActionHandler>();
        private static Dictionary<string, MachineConfig> machineConfigDict = new Dictionary<string, MachineConfig>();
        private static Dictionary<string, Dictionary<string, StateConfig>> stateConfigDict = new Dictionary<string, Dictionary<string, StateConfig>>();

        private static Stack<ActionNode> actionNodePool = new Stack<ActionNode>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loader"></param>
        public static void Init(Func<string, MachineConfig> loader)
        {
            ActionMachineHelper.loader = loader;
        }

        /// <summary>
        /// 获取操作类
        /// </summary>
        /// <param name="type">配置文件类型</param>
        /// <returns>操作类</returns>
        public static IActionHandler GetActionHandler(Type type)
        {
            IActionHandler handler = null;

            if (actionHandlerDict.TryGetValue(type, out handler))
            {
                return handler;
            }

            ActionConfigAttribute config = type.GetCustomAttribute<ActionConfigAttribute>(true);

            Type handlerType = config.handlerType;

            handler = Activator.CreateInstance(handlerType) as IActionHandler;
            if (handler == null)
            {
                throw new RuntimeException($"{handlerType} 类型未继承 {nameof(IActionHandler)} 接口");
            }

            actionHandlerDict.Add(type, handler);

            return handler;
        }

        /// <summary>
        /// 获取状态机配置文件
        /// </summary>
        /// <param name="configName">状态机配置文件名</param>
        /// <returns>配置</returns>
        public static MachineConfig GetMachineConfig(string configName)
        {
            MachineConfig config = null;
            if (machineConfigDict.TryGetValue(configName, out config))
            {
                return config;
            }

            config = loader(configName);
            if (config == null)
            {
                throw new RuntimeException($"状态机配置 {configName} 未找到");
            }
            machineConfigDict.Add(configName, config);

            return config;
        }

        public static StateConfig GetStateConfig(string configName, string stateName)
        {
            StateConfig config;
            Dictionary<string, StateConfig> stateDict;
            if (stateConfigDict.TryGetValue(configName, out stateDict)
            && stateDict.TryGetValue(stateName, out config))
            {
                return config;
            }

            if (stateDict == null)
            {
                stateDict = new Dictionary<string, StateConfig>();
                stateConfigDict.Add(configName, stateDict);
            }

            MachineConfig mConfig = GetMachineConfig(configName);
            config = mConfig.states.Find(t => 0 == string.Compare(stateName, t.stateName));
            if (config == null)
            {
                throw new RuntimeException($"状态机配置 {configName} 中的 {stateName} 未找到");
            }

            stateDict.Add(stateName, config);

            return config;
        }

        /// <summary>
        /// 创建动作节点
        /// </summary>
        /// <returns>节点</returns>
        public static ActionNode CreateActionNode()
        {
            if (actionNodePool.Count > 0)
            {
                return actionNodePool.Pop();
            }

            ActionNode node = new ActionNode();
            return node;
        }

        /// <summary>
        /// 回收动作节点
        /// </summary>
        /// <param name="node">节点</param>
        public static void RecycleActionNode(ActionNode node)
        {
            node.Reset();
            actionNodePool.Push(node);
        }

        #endregion static
    }
}
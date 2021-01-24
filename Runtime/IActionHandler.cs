/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/5 16:01:03
 */

namespace XMLib.AM
{
    /// <summary>
    /// IActionHandler
    /// </summary>
    public interface IActionHandler<ControllerType, FloatType> where FloatType : struct
    {
        void Enter(ActionNode<ControllerType, FloatType> node);

        void Exit(ActionNode<ControllerType, FloatType> node);

        void Update(ActionNode<ControllerType, FloatType> node, FloatType deltaTime);
    }
}
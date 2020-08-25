/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/31 11:25:00
 */

using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// TestActionMachine
    /// </summary>
    [RequireComponent(typeof(AnimatorTest))]
    public class ActionMachineTest : MonoBehaviour
    {
        public TextAsset config;
        public Matrix4x4 localToWorldMatrix => transform.localToWorldMatrix;

        private void Awake() {
            Destroy(gameObject);
        }
    }
}
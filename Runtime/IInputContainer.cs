/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/19 17:26:52
 */

using UnityEngine;

namespace XMLib.AM
{
    public struct InputData
    {
        public int keyCode;
        public byte axisValue;

        public static readonly InputData none = new InputData() { keyCode = 0, axisValue = byte.MaxValue };

        public static bool operator ==(InputData a, InputData b)
        {
            return a.keyCode == b.keyCode && a.axisValue == b.axisValue;
        }

        public static bool operator !=(InputData a, InputData b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return keyCode & axisValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is InputData data))
            {
                return false;
            }

            return this == data;
        }
    }

    /// <summary>
    /// IInputContainer
    /// </summary>
    public interface IInputContainer
    {
        InputData GetRawInput(int id);

        int GetAllKey(int id);

        byte GetAxis(int id);

        bool HasKey(int id, int keyCode, bool fullMatch = false);
    }
}
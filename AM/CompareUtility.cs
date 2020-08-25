/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/20 16:58:44
 */

using System;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// CompareUtility
    /// </summary>
    public static class CompareUtility
    {
        public static bool Length(float a, float b, CompareType compare, bool isAbs = false)
        {
            if (isAbs)
            {//取绝对值
                a = Mathf.Abs(a);
                b = Mathf.Abs(b);
            }

            return Compare(a, b, compare);
        }

        public static bool Compare(object a, object b, CompareType compare)
        {
            int result = ((IComparable)a).CompareTo(b);
            return CheckResult(result, compare);
        }

        public static bool Compare<T>(T a, T b, CompareType compare) where T : IComparable<T>
        {
            int result = a.CompareTo(b);
            return CheckResult(result, compare);
        }

        private static bool CheckResult(int result, CompareType compare)
        {
            switch (compare)
            {
                case CompareType.none:
                    return false;

                case CompareType.Greater:
                    return result > 0;

                case CompareType.Less:
                    return result < 0;

                case CompareType.Equal:
                    return result == 0;

                case CompareType.EqualOrGreater:
                    return result >= 0;

                case CompareType.EqualOrLess:
                    return result <= 0;

                case CompareType.NotEqual:
                    return result != 0;

                default:
                    throw new RuntimeException($"不存在 {compare} 这种比较方式");
            }
        }
    }
}
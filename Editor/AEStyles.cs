/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/1/15 9:52:56
 */

using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// AEStyles
    /// </summary>
    public static class AEStyles
    {
        public static GUIStyle list_tool_bg = "IN Footer";
        public static GUIStyle view_head = "HeaderButton";
        public static GUIStyle view_bg = "hostview";
        public static GUIStyle text_bg = "WhiteBackground";
        public static GUIStyle item_head_normal = "MeTransitionSelect";
        public static GUIStyle item_head_select = "MeTransitionSelectHead";
        public static GUIStyle item_body_normal = new GUIStyle("ScrollViewAlt") { alignment = TextAnchor.MiddleLeft };
        public static GUIStyle item_body_select = new GUIStyle("TE ElementBackground") { alignment = TextAnchor.MiddleLeft };
        public static GUIStyle frame_label = "CN StatusWarn";
        public static GUIStyle box = "HelpBox";
    }
}
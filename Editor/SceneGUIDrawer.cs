/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/31 11:34:28
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// SceneGUIDrawer
    /// </summary>
    public class SceneGUIDrawer
    {
        public ActionEditorWindow win { get; set; }

        public void OnSceneGUI(SceneView sceneView)
        {
            Matrix4x4 localToWorld = (Matrix4x4)win.actionMachine.localToWorldMatrix;

            FrameConfig config = win.currentFrame;
            if (config == null)
            {
                return;
            }

            if (config.attackRanges != null)
            {
                ProcessRanges(config.attackRanges, localToWorld, true, win.attackRangeSelectIndex, new Color(1, 0, 0, 0.25f));
            }

            if (config.stayBodyRange)
            {//保持范围的时候，不可以设置
                List<RangeConfig> bodyRanges = win.FindStayBodyRangeStartWith(win.frameSelectIndex);
                if (bodyRanges != null)
                {
                    ProcessRanges(bodyRanges, localToWorld, false, -1, new Color(0, 0, 1, 0.25f));
                }
            }
            else
            {
                if (config.bodyRanges != null)
                {
                    ProcessRanges(config.bodyRanges, localToWorld, true, win.bodyRangeSelectIndex, new Color(0, 0, 1, 0.25f));
                }
            }
        }

        private void ProcessRanges(List<RangeConfig> ranges, Matrix4x4 localToWorld, bool enableControl, int selectIndex, Color color)
        {
            int length = ranges.Count;
            for (int i = 0; i < length; i++)
            {
                RangeConfig config = ranges[i];
                DrawRange(i, config, localToWorld, color);

                if (enableControl && (win.setting.enableAllControl || (selectIndex == i)))
                {
                    ControlRange(i, config.value, localToWorld);
                }
            }
        }

        private void DrawRange(int index, RangeConfig config, Matrix4x4 localToWorld, Color color)
        {
            HandlesDrawer.H.PushAndSetColor(color);
            HandlesDrawer.H.fillConvexHull = true;
            switch (config.value)
            {
                case RangeConfig.RectItem v:
                    HandlesDrawer.H.DrawRect(v.size, localToWorld * Matrix4x4.Translate(v.offset));
                    break;

                case RangeConfig.CircleItem v:
                    HandlesDrawer.H.DrawCircle(v.radius, localToWorld * Matrix4x4.Translate(v.offset));
                    break;

                case RangeConfig.BoxItem v:

                    HandlesDrawer.H.DrawBox(v.size, localToWorld * Matrix4x4.Translate(v.offset));
                    break;

                case RangeConfig.SphereItem v:
                    HandlesDrawer.H.DrawSphere(v.radius, localToWorld * Matrix4x4.Translate(v.offset));
                    break;
            }
            HandlesDrawer.H.fillConvexHull = false;
            HandlesDrawer.H.PopColor();
        }

        private float FixFloat(float v)
        {
            return (float)Math.Round(v, 3);
        }

        private BoxBoundsHandle boxHandle = new BoxBoundsHandle();
        private SphereBoundsHandle sphereHandle = new SphereBoundsHandle();

        private void ControlRange(int index, RangeConfig.IItem config, Matrix4x4 localToWorld)
        {
            Matrix4x4 worldToLocal = localToWorld.inverse;

            Vector3 worldPos = Vector3.zero;
            Vector3 worldSize = Vector3.zero;
            Quaternion worldRotate = Quaternion.identity;

            switch (config)
            {
                case RangeConfig.RectItem v:
                    worldPos = localToWorld.MultiplyPoint(v.offset);
                    worldSize = (Vector2)v.size;
                    break;

                case RangeConfig.CircleItem v:
                    worldPos = localToWorld.MultiplyPoint(v.offset);
                    worldSize = new Vector2((float)v.radius, 0f);
                    break;

                case RangeConfig.BoxItem v:
                    worldPos = localToWorld.MultiplyPoint(v.offset);
                    worldSize = v.size;
                    break;

                case RangeConfig.SphereItem v:
                    worldPos = localToWorld.MultiplyPoint(v.offset);
                    worldSize = new Vector2((float)v.radius, 0f);
                    break;

                default:
                    return;
            }

            float handleSize = HandleUtility.GetHandleSize(worldPos);

            //draw handle
            bool useOffset = false;
            bool useSize = false;

            switch (Tools.current)
            {
                case Tool.View:
                    break;

                case Tool.Move:
                    worldPos = Handles.DoPositionHandle(worldPos, worldRotate);
                    useOffset = true;
                    break;

                case Tool.Scale:
                    worldSize = Handles.DoScaleHandle(worldSize, worldPos, worldRotate, handleSize);
                    useSize = true;
                    break;

                case Tool.Transform:
                    Handles.TransformHandle(ref worldPos, Quaternion.identity, ref worldSize);
                    useSize = true;
                    useOffset = true;
                    break;

                case Tool.Rect:

                    switch (config)
                    {
                        case RangeConfig.RectItem v:
                            {
                                boxHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
                                boxHandle.center = worldPos;
                                boxHandle.size = worldSize;
                                boxHandle.DrawHandle();
                                worldPos = boxHandle.center;
                                worldSize = boxHandle.size;
                                useOffset = true;
                                useSize = true;
                                break;
                            }

                        case RangeConfig.CircleItem v:
                            {
                                sphereHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
                                sphereHandle.center = worldPos;
                                sphereHandle.radius = worldSize.x;
                                sphereHandle.DrawHandle();
                                worldPos = sphereHandle.center;
                                worldSize.x = sphereHandle.radius;
                                useOffset = true;
                                useSize = true;
                                break;
                            }
                        case RangeConfig.BoxItem v:
                            {
                                boxHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.Z;
                                boxHandle.center = worldPos;
                                boxHandle.size = worldSize;
                                boxHandle.DrawHandle();
                                worldPos = boxHandle.center;
                                worldSize = boxHandle.size;
                                useOffset = true;
                                useSize = true;
                                break;
                            }
                        case RangeConfig.SphereItem v:
                            {
                                sphereHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.Z;
                                sphereHandle.center = worldPos;
                                sphereHandle.radius = worldSize.x;
                                sphereHandle.DrawHandle();
                                worldPos = sphereHandle.center;
                                worldSize.x = sphereHandle.radius;
                                useOffset = true;
                                useSize = true;
                                break;
                            }
                    }

                    break;
            }

            Func<Vector3> getOffset = () =>
            {
                Vector3 offset = worldToLocal.MultiplyPoint(worldPos);
                offset.x = FixFloat(offset.x);
                offset.y = FixFloat(offset.y);
                offset.z = FixFloat(offset.z);
                return offset;
            };

            Func<Vector3> getSize = () =>
             {
                 Vector3 size = new Vector3(FixFloat(worldSize.x), FixFloat(worldSize.y), FixFloat(worldSize.z));
                 return size;
             };

            Func<float> getRadius = () =>
            {
                float radius = FixFloat(worldSize.magnitude);
                return radius;
            };

            switch (config)
            {
                case RangeConfig.RectItem v:
                    v.offset = useOffset ? (Vector2)getOffset() : v.offset;
                    v.size = useSize ? (Vector2)getSize() : v.size;
                    break;

                case RangeConfig.CircleItem v:
                    v.offset = useOffset ? (Vector2)getOffset() : v.offset;
                    v.radius = useSize ? getRadius() : v.radius;
                    break;

                case RangeConfig.BoxItem v:
                    v.offset = useOffset ? getOffset() : v.offset;
                    v.size = useSize ? getSize() : v.size;
                    break;

                case RangeConfig.SphereItem v:
                    v.offset = useOffset ? getOffset() : v.offset;
                    v.radius = useSize ? getRadius() : v.radius;
                    break;
            }
        }
    }
}
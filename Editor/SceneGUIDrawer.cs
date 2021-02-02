/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/31 11:34:28
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// SceneGUIDrawer
    /// </summary>
    public class SceneGUIDrawer<ControllerType, FloatType> where FloatType : struct
    {
        public ActionEditorWindow<ControllerType, FloatType> win { get; set; }

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
            Matrix4x4 localToWorldNoScale = Matrix4x4.TRS(localToWorld.MultiplyPoint3x4(Vector3.zero), localToWorld.rotation, Vector3.one);

            Matrix4x4 oldMat = Handles.matrix;
            Handles.matrix = localToWorldNoScale;

            int length = ranges.Count;
            for (int i = 0; i < length; i++)
            {
                RangeConfig config = ranges[i];
                DrawRange(i, config, color);

                if (enableControl && (win.setting.enableAllControl || (selectIndex == i)))
                {
                    ControlRange(i, config.value);
                }
            }

            Handles.matrix = oldMat;
        }

        private void DrawRange(int index, RangeConfig config, Color color)
        {
            HandlesDrawer.H.PushAndSetColor(color);
            HandlesDrawer.H.fillColor = true;
            switch (config.value)
            {
                case Ranges.RectItem v:
                    HandlesDrawer.H.DrawRect(v.size, Matrix4x4.Translate(v.offset));
                    break;

                case Ranges.CircleItem v:
                    HandlesDrawer.H.DrawCircle(v.radius, Matrix4x4.Translate(v.offset));
                    break;

                case Ranges.BoxItem v:
                    HandlesDrawer.H.DrawBox(v.size, Matrix4x4.Translate(v.offset));
                    break;

                case Ranges.SphereItem v:
                    HandlesDrawer.H.DrawSphere(v.radius, Matrix4x4.Translate(v.offset));
                    break;
            }
            HandlesDrawer.H.fillColor = false;
            HandlesDrawer.H.PopColor();
        }

        private float FixFloat(float v)
        {
            return (float)Math.Round(v, 3);
        }

        private BoxBoundsHandle boxHandle = new BoxBoundsHandle();
        private SphereBoundsHandle sphereHandle = new SphereBoundsHandle();

        private void ControlRange(int index, Ranges.IItem config)
        {
            Vector3 offset = Vector3.zero;
            Vector3 size = Vector3.zero;

            switch (config)
            {
                case Ranges.RectItem v:
                    offset = v.offset;
                    size = v.size;
                    break;

                case Ranges.CircleItem v:
                    offset = v.offset;
                    size = new Vector3(v.radius, 0f, 0f);
                    break;

                case Ranges.BoxItem v:
                    offset = v.offset;
                    size = v.size;
                    break;

                case Ranges.SphereItem v:
                    offset = v.offset;
                    size = new Vector2(v.radius, 0f);
                    break;

                default:
                    return;
            }

            //draw handle =========================================

            float handleSize = HandleUtility.GetHandleSize(offset);

            switch (Tools.current)
            {
                case Tool.View:
                    break;

                case Tool.Move:
                    offset = Handles.DoPositionHandle(offset, Quaternion.identity);
                    break;

                case Tool.Scale:
                    size = Handles.DoScaleHandle(size, offset, Quaternion.identity, handleSize);
                    break;

                case Tool.Transform:
                    Handles.TransformHandle(ref offset, Quaternion.identity, ref size);
                    break;

                case Tool.Rect:

                    switch (config)
                    {
                        case Ranges.RectItem v:
                            {
                                boxHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
                                boxHandle.center = offset;
                                boxHandle.size = size;
                                boxHandle.DrawHandle();
                                offset = boxHandle.center;
                                size = boxHandle.size;
                                break;
                            }

                        case Ranges.CircleItem v:
                            {
                                sphereHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
                                sphereHandle.center = offset;
                                sphereHandle.radius = size.x;
                                sphereHandle.DrawHandle();
                                offset = sphereHandle.center;
                                size.x = sphereHandle.radius;
                                break;
                            }
                        case Ranges.BoxItem v:
                            {
                                boxHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.Z;
                                boxHandle.center = offset;
                                boxHandle.size = size;
                                boxHandle.DrawHandle();
                                offset = boxHandle.center;
                                size = boxHandle.size;
                                break;
                            }
                        case Ranges.SphereItem v:
                            {
                                sphereHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.Z;
                                sphereHandle.center = offset;
                                sphereHandle.radius = size.x;
                                sphereHandle.DrawHandle();
                                offset = sphereHandle.center;
                                size.x = sphereHandle.radius;
                                break;
                            }
                    }

                    break;
            }

            //===============================================

            Func<Vector3> getOffset = () => new Vector3(FixFloat(offset.x), FixFloat(offset.y), FixFloat(offset.z));
            Func<Vector3> getSize = () => new Vector3(FixFloat(size.x), FixFloat(size.y), FixFloat(size.z));
            Func<float> getRadius = () => FixFloat(size.magnitude);

            switch (config)
            {
                case Ranges.RectItem v:
                    v.offset = getOffset();
                    v.size = getSize();
                    break;

                case Ranges.CircleItem v:
                    v.offset = getOffset();
                    v.radius = getRadius();
                    break;

                case Ranges.BoxItem v:
                    v.offset = getOffset();
                    v.size = getSize();
                    break;

                case Ranges.SphereItem v:
                    v.offset = getOffset();
                    v.radius = getRadius();
                    break;
            }
        }
    }
}
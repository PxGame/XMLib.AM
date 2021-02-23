/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 17:15:46
 */

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    /// <summary>
    /// MenuView
    /// </summary>
    [Serializable]
    public class MenuView : IView
    {
        public override bool checkConfig => false;
        public override string title => string.Empty;
        public override bool useAre => true;
        private string selectConfigName;

        protected override void OnGUI(Rect rect)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("关闭弹出", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                foreach (var view in win.views)
                {
                    view.HidePopWindow();
                }
            }

            if (!win.isRunning)
            {
                EditorMode();
            }
            else
            {
                PlayMode();
            }

            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }

        private void EditorMode()
        {
            Event evt = Event.current;

            if (GUILayout.Button("保存", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                SaveConfig();
            }
            if (GUILayout.Button("保存到", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                SaveConfigToSelect(win.config);
            }
            if (GUILayout.Button("还原修改", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                ResetConfig();
            }
            if (GUILayout.Button("拷贝配置", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                CopyConfig();
            }
            if (GUILayout.Button("粘贴配置", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                PasteConfig();
            }

            win.setting.showView = EditorGUILayoutEx.DrawObject("显示", win.setting.showView);

            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();
            win.actionMachineObj = (GameObject)EditorGUILayout.ObjectField(win.actionMachineObj, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck() && win.actionMachineObj != null)
            {
                if (win.actionMachineObj != null)
                {
                    win.UpdateTarget(win.actionMachineObj);
                }
            }

            EditorGUI.BeginChangeCheck();
            win.configAsset = (TextAsset)EditorGUILayout.ObjectField(win.configAsset, typeof(TextAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                if (win.configAsset != null)
                {
                    win.UpdateConfig(win.configAsset);
                }
            }

            if (GUILayout.Button("创建", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                CreateNew();
            }
        }

        private void PlayMode()
        {
            win.setting.showView = EditorGUILayoutEx.DrawObject("显示", win.setting.showView);
            GUILayout.FlexibleSpace();

            string[] configNames = ActionMachineHelper.loadedConfig.Keys.ToArray();
            if (configNames == null || configNames.Length == 0)
            {
                selectConfigName = string.Empty;

                GUILayout.Label("当前没有已加载的配置文件");
                return;
            }

            bool isInit = true;
            int index = 0;
            if (!string.IsNullOrEmpty(selectConfigName))
            {
                index = Array.FindIndex(configNames, t => string.Compare(t, selectConfigName) == 0);
                if (index < 0)
                {
                    index = 0;
                }
            }
            else
            {
                isInit = false;
            }

            GUILayout.Label("已加载配置文件");

            EditorGUI.BeginChangeCheck();
            index = EditorGUILayout.Popup(GUIContent.none, index, configNames, GUILayout.Width(200f));
            if (EditorGUI.EndChangeCheck() || !isInit)
            {
                selectConfigName = configNames[index];

                MachineConfig config = ActionMachineHelper.loadedConfig[selectConfigName];
                win.config = config;
            }

            if (GUILayout.Button("覆盖到", GUILayout.Width(80)))
            {
                GUI.FocusControl(null);
                SaveConfigToSelect(win.config);
            }
            if (GUILayout.Button("另存到", GUILayout.Width(100)))
            {
                GUI.FocusControl(null);
                CopyToNew(win.config);
            }
        }

        private void PasteConfig()
        {
            if (null == win.configAsset)
            {
                throw new RuntimeException($"未选择配置资源");
            }

            MachineConfig config = null;

            try
            {
                string data = EditorGUIUtility.systemCopyBuffer;
                config = DataUtility.FromJson<MachineConfig>(data);
                if (null == config)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("剪贴板数据不匹配");
                return;
            }

            win.config = config;
        }

        private void CopyConfig()
        {
            if (!win.isConfigValid)
            {
                throw new RuntimeException($"未选择配置资源");
            }

            string data = DataUtility.ToJson(win.config);
            EditorGUIUtility.systemCopyBuffer = data;
        }

        private void ResetConfig()
        {
            win.UpdateConfig(win.configAsset);
        }

        public void SaveConfig()
        {
            if (!win.isConfigValid)
            {
                throw new RuntimeException($"未选择配置资源");
            }

            string path = AssetDatabase.GetAssetPath(win.configAsset);
            string data = DataUtility.ToJson(win.config);
            File.WriteAllText(path, data);
            Debug.Log($"当前配置已保存 : {path}");
            AssetDatabase.Refresh();
        }

        private void CreateNew()
        {
            string dir = EditorUtilityEx.GetSelectDirectory();
            string filePath = EditorUtilityEx.ValidFilePath(Path.Combine(dir, "MachineConfig.bytes"));
            string data = DataUtility.ToJson(new MachineConfig());
            File.WriteAllText(filePath, data);
            Debug.Log($"配置已创建到 : {filePath}");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            win.configAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
            win.UpdateConfig(win.configAsset);
        }

        private void CopyToNew(MachineConfig config)
        {
            string dir = EditorUtilityEx.GetSelectDirectory();
            string filePath = EditorUtilityEx.ValidFilePath(Path.Combine(dir, "MachineConfig.bytes"));
            string data = DataUtility.ToJson(config);
            File.WriteAllText(filePath, data);
            AssetDatabase.Refresh();

            Debug.Log($"配置已拷贝到 : {filePath}");
        }

        private void SaveConfigToSelect(MachineConfig config)
        {
            string filePath = EditorUtilityEx.GetSelectFile(".bytes");
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("选择的文件无效，写入失败");
                return;
            }

            if (!EditorUtility.DisplayDialog("提示", $"是否将当前配置覆盖到选择文件?\n{filePath}", "是", "否"))
            {
                return;
            }

            string data = DataUtility.ToJson(config);
            File.WriteAllText(filePath, data);
            AssetDatabase.Refresh();

            Debug.Log($"配置已覆盖到 : {filePath}");
        }

        public override void OnUpdate()
        {
        }
    }
}
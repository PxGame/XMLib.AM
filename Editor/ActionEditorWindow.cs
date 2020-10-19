/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:10:56
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace XMLib.AM
{
    /// <summary>
    /// ActionEditorWindow
    /// </summary>
    public class ActionEditorWindow : EditorWindow
    {
        [MenuItem("XMLib/动作编辑")]
        public static void ShowEditor()
        {
            GetWindow<ActionEditorWindow>();
        }

        public static void ShowEditor(GameObject target, TextAsset config)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("编辑器不能在运行时打开");
                return;
            }

            var win = GetWindow<ActionEditorWindow>();
            if (win.configAsset != null)
            {
                if (win.configAsset == config)
                {// 如果当前已打开的窗口相同，则focus,并直接返回
                    win.Focus();
                    return;
                }
                else
                {
                    //如果不相同，则创建一个新的窗口
                    win = CreateWindow<ActionEditorWindow>();
                    win.Show();
                }
            }

            //更新参数
            win.UpdateTarget(target);
            win.UpdateConfig(config);
        }

        [Flags]
        public enum ViewType
        {
            None = 0b0000_0000,

            GlobalAction = 0b0000_0001,
            State = 0b0000_0010,
            StateSet = 0b0000_0100,
            Action = 0b0000_1000,
            Tool = 0b0001_0000,
            Other = 0b0010_0000,
        }

        /// <summary>
        /// 编辑器配置
        /// </summary>
        [Serializable]
        public class Setting
        {
            public int stateSelectIndex = -1;
            public int attackRangeSelectIndex = -1;
            public int bodyRangeSelectIndex = -1;
            public int actionSelectIndex = -1;
            public int globalActionSelectIndex = -1;
            public int frameSelectIndex = -1;
            public bool enableAllControl = false;
            public bool enableQuickKey = false;

            public ViewType showView;

            public float frameRate => 0.033f;

            public Vector2 otherViewScrollPos = Vector2.zero;

            public float frameWidth = 40;
            public float frameListViewRectHeight = 200f;

            public ToolView.Setting toolView = new ToolView.Setting();
        }

        [NonSerialized] public readonly ActionListView actionListView;
        [NonSerialized] public readonly ActionSetView actionSetView;
        [NonSerialized] public readonly GlobalActionListView globalActionListView;
        [NonSerialized] public readonly GlobalActionSetView globalActionSetView;
        [NonSerialized] public readonly AttackRangeListView attackRangeListView;
        [NonSerialized] public readonly BodyRangeListView bodyRangeListView;
        [NonSerialized] public readonly FrameListView frameListView;
        [NonSerialized] public readonly StateListView stateListView;
        [NonSerialized] public readonly StateSetView stateSetView;
        [NonSerialized] public readonly MenuView menuView;
        [NonSerialized] public readonly ToolView toolView;

        [SerializeReference]
        private List<IView> views = new List<IView>();

        private readonly SceneGUIDrawer guiDrawer;
        private readonly QuickButtonHandler quickButtonHandler;

        #region style

        private readonly float space = 3f;
        private readonly float scrollHeight = 13f;
        private readonly float menuViewRectHeight = 26f;
        private readonly float stateListViewRectWidth = 150f;
        private readonly float stateSetViewRectWidth = 200f;
        private readonly float bodyRangeListViewRectWidth = 200f;
        private readonly float attackRangeListViewRectWidth = 200f;
        private readonly float actionListViewRectWidth = 180f;
        private readonly float actionSetViewRectWidth = 300f;
        private readonly float globalActionListViewRectWidth = 180f;
        private readonly float globalActionSetViewRectWidth = 300f;
        private readonly float toolViewRectWidth = 200f;
        public float frameWidth => setting.frameWidth;
        public float frameListViewRectHeight => setting.frameListViewRectHeight;

        #endregion style

        #region data

        #region raw data

        protected static string settingPath = "XMLib.AM.ActionEditorWindow";

        public Setting setting = new Setting();

        public bool actionMachineDirty = false;

        public bool isRunning => EditorApplication.isPlaying;

        public GameObject actionMachineObj = null;
        public ActionMachineTest actionMachine = null;
        public TextAsset configAsset = null;
        public MachineConfig config;

        #endregion raw data

        public bool isConfigValid => config != null && (isRunning || configAsset != null);
        public bool isActionMachineValid => actionMachine != null;
        public bool isCurrentAnimationClipValid => null != GetCurrentAnimationClip();

        public int stateSelectIndex
        {
            get
            {
                CheckSelectIndex(ref setting.stateSelectIndex, currentStates);
                return setting.stateSelectIndex;
            }
            set
            {
                int oldIndex = setting.stateSelectIndex;
                setting.stateSelectIndex = value;
                CheckSelectIndex(ref setting.stateSelectIndex, currentStates);
                if (oldIndex != value && oldIndex != setting.stateSelectIndex)
                {//当前帧发生改变
                    actionMachineDirty = true;
                }
            }
        }

        public int attackRangeSelectIndex
        {
            get
            {
                CheckSelectIndex(ref setting.attackRangeSelectIndex, currentAttackRanges);
                return setting.attackRangeSelectIndex;
            }
            set
            {
                setting.attackRangeSelectIndex = value;
                CheckSelectIndex(ref setting.attackRangeSelectIndex, currentAttackRanges);
            }
        }

        public int bodyRangeSelectIndex
        {
            get
            {
                CheckSelectIndex(ref setting.bodyRangeSelectIndex, currentBodyRanges);
                return setting.bodyRangeSelectIndex;
            }
            set
            {
                setting.bodyRangeSelectIndex = value;
                CheckSelectIndex(ref setting.bodyRangeSelectIndex, currentBodyRanges);
            }
        }

        public int actionSelectIndex
        {
            get
            {
                CheckSelectIndex(ref setting.actionSelectIndex, currentActions);
                return setting.actionSelectIndex;
            }
            set
            {
                setting.actionSelectIndex = value;
                CheckSelectIndex(ref setting.actionSelectIndex, currentActions);
            }
        }

        public int globalActionSelectIndex
        {
            get
            {
                CheckSelectIndex(ref setting.globalActionSelectIndex, currentGlobalActions);
                return setting.globalActionSelectIndex;
            }
            set
            {
                setting.globalActionSelectIndex = value;
                CheckSelectIndex(ref setting.globalActionSelectIndex, currentGlobalActions);
            }
        }

        public int frameSelectIndex
        {
            get
            {
                CheckSelectIndex(ref setting.frameSelectIndex, currentFrames);
                return setting.frameSelectIndex;
            }
            set
            {
                int oldIndex = setting.frameSelectIndex;
                setting.frameSelectIndex = value;
                CheckSelectIndex(ref setting.frameSelectIndex, currentFrames);
                if (oldIndex != value && oldIndex != setting.frameSelectIndex)
                {//当前帧发生改变
                    actionMachineDirty = true;
                }
            }
        }

        public List<StateConfig> currentStates => config?.states;
        public List<FrameConfig> currentFrames => currentState?.frames;
        public List<RangeConfig> currentBodyRanges => currentFrame?.bodyRanges;
        public List<RangeConfig> currentAttackRanges => currentFrame?.attackRanges;
        public List<object> currentActions => currentState?.actions;
        public List<object> currentGlobalActions => config?.globalActions;

        public int currentStateCount => currentStates?.Count ?? -1;
        public int currentFrameCount => currentFrames?.Count ?? -1;
        public int currentBodyRangeCount => currentBodyRanges?.Count ?? -1;
        public int currentAttackRangeCount => currentAttackRanges?.Count ?? -1;
        public int currentActionCount => currentActions?.Count ?? -1;
        public int currentGlobalActionCount => currentGlobalActions?.Count ?? -1;

        public StateConfig currentState => GetSelectItem(stateSelectIndex, currentStates);
        public FrameConfig currentFrame => GetSelectItem(frameSelectIndex, currentFrames);
        public RangeConfig currentBodyRange => GetSelectItem(bodyRangeSelectIndex, currentBodyRanges);
        public RangeConfig currentAttackRange => GetSelectItem(attackRangeSelectIndex, currentAttackRanges);
        public object currentAction => GetSelectItem(actionSelectIndex, currentActions);
        public object currentGlobalAction => GetSelectItem(globalActionSelectIndex, currentGlobalActions);

        public List<RangeConfig> FindStayBodyRangeStartWith(int frameIndex, bool copyNew = false)
        {
            FrameConfig config = null;
            StateConfig state = currentState;
            while (frameIndex >= 0)
            {
                FrameConfig frame = state.frames[frameIndex];
                if (!frame.stayBodyRange)
                {
                    config = frame;
                    break;
                }
                frameIndex--;
            }

            List<RangeConfig> result = copyNew ? config?.CopyBodyRanges() : config?.bodyRanges;
            return result;
        }

        public List<RangeConfig> FindStayBodyRangeFromCurrent(bool copyNew = false)
        {
            return FindStayBodyRangeStartWith(frameSelectIndex, copyNew);
        }

        public void CopyBodyRangeToCurrentFrameIfStay()
        {
            FrameConfig config = currentFrame;
            if (config == null || !config.stayBodyRange)
            {
                return;
            }

            List<RangeConfig> target = FindStayBodyRangeStartWith(frameSelectIndex);
            if (target == null)
            {
                config.bodyRanges = new List<RangeConfig>();
                return;
            }

            config.CopyBodyRangeFrom(target);
            config.stayBodyRange = false;
        }

        private int CheckSelectIndex<T>(ref int index, IList<T> list)
        {
            return index = list == null ? -1 : Mathf.Clamp(index, -1, list.Count - 1);
        }

        private T GetSelectItem<T>(int index, IList<T> list) where T : class
        {
            if (index < 0 || null == list || list.Count == 0)
            {
                return null;
            }
            return list[index];
        }

        #endregion data

        public ActionEditorWindow()
        {
            globalActionListView = CreateView<GlobalActionListView>();
            globalActionSetView = CreateView<GlobalActionSetView>();
            actionListView = CreateView<ActionListView>();
            actionSetView = CreateView<ActionSetView>();
            attackRangeListView = CreateView<AttackRangeListView>();
            bodyRangeListView = CreateView<BodyRangeListView>();
            frameListView = CreateView<FrameListView>();
            stateListView = CreateView<StateListView>();
            stateSetView = CreateView<StateSetView>();
            menuView = CreateView<MenuView>();
            toolView = CreateView<ToolView>();

            guiDrawer = new SceneGUIDrawer() { win = this };
            quickButtonHandler = new QuickButtonHandler() { win = this };
        }

        private T CreateView<T>() where T : IView, new()
        {
            T obj = new T();
            obj.win = this;
            views.Add(obj);
            return obj;
        }

        ~ActionEditorWindow()
        {
        }

        private void Awake()
        {
        }

        private void OnDestroy()
        {
        }

        private void OnEnable()
        {
            //加载配置
            string data = EditorUserSettings.GetConfigValue(settingPath);
            if (!string.IsNullOrEmpty(data))
            {
                EditorJsonUtility.FromJsonOverwrite(data, setting);
            }
            //

            autoRepaintOnSceneChange = true;

            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            //保存配置
            string data = EditorJsonUtility.ToJson(setting, false);
            EditorUserSettings.SetConfigValue(settingPath, data);
            //
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            /*
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:

                    if (isConfigValid && EditorUtility.DisplayDialog("动作编辑器提示", "是否立即保存配置？\n如果未保存，运行时则会丢失当前修改。", "是", "否"))
                    {
                        menuView.SaveConfig();
                    }

                    actionMachineObj = null;
                    actionMachine = null;
                    configAsset = null;
                    config = null;
                    break;
            }
            */
        }

        private void OnGUI()
        {
            Check();

            Undo.RecordObject(this, "ActionEditorWindow");

            AEStyles.Begin();
            Draw();
            AEStyles.End();
            UpdateActionMachine();

            EventProcess();

            quickButtonHandler.OnGUI();
        }

        private void EventProcess()
        {
            Rect rect = position;

            Event evt = Event.current;
            if (!rect.Contains(Event.current.mousePosition))
            {
                return;
            }
        }

        private void Check()
        {
            if (!isActionMachineValid)
            {
                actionMachineObj = null;
                actionMachine = null;
            }

            if (!isConfigValid)
            {
                configAsset = null;
                config = null;
            }

            //更新标题
            this.titleContent = new GUIContent(configAsset != null ? $"编辑-{configAsset.name}" : $"动作编辑器");
        }

        public void UpdateTarget(GameObject target)
        {
            if (target == null)
            {
                throw new RuntimeException($"未选择目标");
            }
            actionMachineObj = target;
            actionMachine = target.GetComponent<ActionMachineTest>();
            if (actionMachine == null)
            {
                actionMachineObj = null;
                throw new RuntimeException($"目标不存在{nameof(ActionMachineTest)}脚本");
            }
        }

        public void UpdateConfig(TextAsset config)
        {
            if (config == null)
            {
                throw new RuntimeException($"未选择配置资源");
            }

            this.configAsset = config;
            this.config = DataUtility.FromJson<MachineConfig>(config.text);
            if (this.config == null)
            {
                throw new RuntimeException($"配置资源解析失败");
            }
        }

        private void Update()
        {
            foreach (var view in views)
            {
                view.OnUpdate();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isConfigValid || actionMachine == null)
            {
                return;
            }

            Undo.RecordObject(this, "ActionEditorWindow");

            AEStyles.Begin();
            guiDrawer.OnSceneGUI(sceneView);
            quickButtonHandler.OnSceneGUI(sceneView);
            AEStyles.End();

            sceneView.Repaint();
            Repaint();
        }

        #region Anima

        private void UpdateActionMachine()
        {
            if (!actionMachineDirty || !isActionMachineValid)
            {
                return;
            }
            actionMachineDirty = false;

            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            AnimationClip clip = GetCurrentAnimationClip();
            int frameIndex = frameSelectIndex;
            if (clip == null || frameIndex < 0)
            {
                return;
            }

            float time = frameIndex * setting.frameRate;

            var state = currentState;

            Animator animator = GetAnimator();
            clip.SampleAnimation(animator.gameObject, time);
        }

        public Animator GetAnimator()
        {
            if (null == actionMachineObj)
            {
                return null;
            }

            Animator animator = actionMachineObj.GetComponent<Animator>();
            if (null != animator)
            {
                return animator;
            }

            animator = actionMachineObj.GetComponentInChildren<Animator>();

            return animator;
        }

        public AnimationClip GetCurrentAnimationClip()
        {
            Animator animator = GetAnimator();
            var state = currentState;

            if (animator == null || state == null || string.IsNullOrEmpty(state.defaultAnimaName))
            {
                return null;
            }

            return Array.Find(animator.runtimeAnimatorController.animationClips, t => string.Compare(state.defaultAnimaName, t.name) == 0);
        }

        #endregion Anima

        #region Draw view

        private static string copyData;
        private static Type copyDataType;

        private void DrawView(IView view, Rect rect, bool checkConfig = true)
        {
            Rect contentRect = rect;

            if (!string.IsNullOrEmpty(view.title))
            {
                float titleHeight = 18f;
                Rect titleRect = new Rect(rect.x, rect.y, rect.width, titleHeight);
                contentRect = new Rect(rect.x, rect.y + titleHeight, rect.width, rect.height - titleHeight);

                GUILayout.BeginArea(titleRect);

                GUILayout.BeginHorizontal();
                GUILayout.Label(view.title, AEStyles.view_head);

                if (view is IDataView dataView)
                {
                    if (GUILayout.Button("C", AEStyles.view_head, GUILayout.Width(20)))
                    {
                        GUI.FocusControl(null);
                        object data = dataView.CopyData();
                        if (data != null)
                        {
                            copyDataType = data.GetType();
                            copyData = DataUtility.ToJson(data, copyDataType);
                        }
                    }

                    if (GUILayout.Button("P", AEStyles.view_head, GUILayout.Width(20)))
                    {
                        GUI.FocusControl(null);
                        if (copyData != null)
                        {
                            object data = DataUtility.FromJson(copyData, copyDataType);
                            if (data != null)
                            {
                                dataView.PasteData(data);
                            }
                        }
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }

            GUI.Box(contentRect, GUIContent.none, AEStyles.view_bg);

            if (!checkConfig || isConfigValid)
            {
                if (view.useAre)
                {
                    GUILayout.BeginArea(contentRect);
                    view.OnGUI(contentRect);
                    GUILayout.EndArea();
                }
                else
                {
                    view.OnGUI(contentRect);
                }
            }
        }

        private void Draw()
        {
            #region calc size

            Rect rect = this.position;
            rect.position = Vector2.zero;

            float startPosX = rect.x;
            float startPosY = rect.y;
            float height = rect.height;
            float width = rect.width;

            Rect menuViewRect = Rect.zero;
            Rect toolViewRect = Rect.zero;
            Rect globalActionListViewRect = Rect.zero;
            Rect globalActionSetViewRect = Rect.zero;
            Rect stateListViewRect = Rect.zero;
            Rect frameListViewRect = Rect.zero;
            Rect bodyRangeListViewRect = Rect.zero;
            Rect attackRangeListViewRect = Rect.zero;
            Rect actionSetViewRect = Rect.zero;
            Rect actionListViewRect = Rect.zero;
            Rect stateSetViewRect = Rect.zero;

            menuViewRect = new Rect(
               startPosX + space,
               startPosY + space,
               rect.width - space,
               menuViewRectHeight - space);
            startPosY += menuViewRectHeight;
            height -= menuViewRectHeight;

            if ((setting.showView & ViewType.GlobalAction) != 0)
            {
                globalActionListViewRect = new Rect(
                   startPosX + space,
                   startPosY + space,
                   globalActionListViewRectWidth - space,
                   height - space * 2);
                startPosX += globalActionListViewRectWidth;
                width -= globalActionListViewRectWidth;

                globalActionSetViewRect = new Rect(
                   startPosX + space,
                   startPosY + space,
                   globalActionSetViewRectWidth - space,
                   height - space * 2);
                startPosX += globalActionSetViewRectWidth;
                width -= globalActionSetViewRectWidth;
            }

            if ((setting.showView & ViewType.State) != 0)
            {
                stateListViewRect = new Rect(
               startPosX + space,
               startPosY + space,
               stateListViewRectWidth - space,
               height - space * 2);
                startPosX += stateListViewRectWidth;
                width -= stateListViewRectWidth;
            }

            frameListViewRect = new Rect(
               startPosX + space,
               startPosY + space,
               width - space,
                frameListViewRectHeight - space);
            startPosY += frameListViewRectHeight;
            height -= frameListViewRectHeight;

            float itemHeight = height - scrollHeight;
            float nextPosX = startPosX;
            float nextPosY = startPosY;

            if ((setting.showView & ViewType.Tool) != 0)
            {
                toolViewRect = new Rect(
                    nextPosX + space,
                    nextPosY + space,
                    toolViewRectWidth - space,
                    itemHeight - space * 2);
                nextPosX += toolViewRectWidth;
            }

            if ((setting.showView & ViewType.StateSet) != 0)
            {
                stateSetViewRect = new Rect(
                    nextPosX + space,
                    nextPosY + space,
                    stateSetViewRectWidth - space,
                    itemHeight - space * 2);
                nextPosX += stateSetViewRectWidth;
            }

            if ((setting.showView & ViewType.Action) != 0)
            {
                actionListViewRect = new Rect(
                    nextPosX + space,
                    nextPosY + space,
                    actionListViewRectWidth - space,
                    itemHeight - space * 2);
                nextPosX += actionListViewRectWidth;

                actionSetViewRect = new Rect(
                    nextPosX + space,
                    nextPosY + space,
                    actionSetViewRectWidth - space,
                    itemHeight - space * 2);
                nextPosX += actionSetViewRectWidth;
            }

            if ((setting.showView & ViewType.Other) != 0)
            {
                attackRangeListViewRect = new Rect(
                    nextPosX + space,
                    nextPosY + space,
                    attackRangeListViewRectWidth - space,
                    itemHeight - space * 2);
                nextPosX += attackRangeListViewRectWidth;

                bodyRangeListViewRect = new Rect(
                    nextPosX + space,
                    nextPosY + space,
                    bodyRangeListViewRectWidth - space,
                    itemHeight - space * 2);
                nextPosX += bodyRangeListViewRectWidth;
            }

            #endregion calc size

            #region draw

            DrawView(menuView, menuViewRect, false);
            DrawView(frameListView, frameListViewRect);

            if ((setting.showView & ViewType.State) != 0)
            {
                DrawView(stateListView, stateListViewRect);
            }

            if ((setting.showView & ViewType.GlobalAction) != 0)
            {
                DrawView(globalActionSetView, globalActionSetViewRect);
                DrawView(globalActionListView, globalActionListViewRect);
            }

            Rect position = new Rect(startPosX + space, startPosY, width - space, height);
            Rect view = new Rect(startPosX + space, startPosY, nextPosX - startPosX - space, itemHeight);
            setting.otherViewScrollPos = GUI.BeginScrollView(position, setting.otherViewScrollPos, view, true, false);

            if ((setting.showView & ViewType.StateSet) != 0)
            {
                DrawView(stateSetView, stateSetViewRect);
            }

            if ((setting.showView & ViewType.Tool) != 0)
            {
                DrawView(toolView, toolViewRect);
            }

            if ((setting.showView & ViewType.Action) != 0)
            {
                DrawView(actionListView, actionListViewRect);
                DrawView(actionSetView, actionSetViewRect);
            }

            if ((setting.showView & ViewType.Other) != 0)
            {
                DrawView(attackRangeListView, attackRangeListViewRect);
                DrawView(bodyRangeListView, bodyRangeListViewRect);
            }
            GUI.EndScrollView();

            #endregion draw
        }

        #endregion Draw view
    }
}
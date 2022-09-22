using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.DeviceSimulator;

namespace UnityEditor.DeviceSimulation
{
    [EditorWindowTitle(title = "Simulator")]
    internal class SimulatorWindow : PlayModeView, IHasCustomMenu, ISerializationCallbackReceiver
    {
        private static List<SimulatorWindow> s_SimulatorInstances = new List<SimulatorWindow>();
        private bool m_DeviceListDirty;
        private bool m_RestartSimulation;

        [SerializeField] private SimulatorState m_SimulatorState;
        private SimulationState m_State = SimulationState.Enabled;
        private DeviceSimulatorMain m_Main;

        [MenuItem("Window/General/Device Simulator", false, 2000)]
        public static void ShowWindow()
        {
            SimulatorWindow window = GetWindow<SimulatorWindow>();
            window.Show();
        }

        void OnEnable()
        {
            var titleImagePath = $"packages/com.unity.device-simulator/Editor/SimulatorResources/Icons/{(EditorGUIUtility.isProSkin ? "d_" : "")}UnityEditor.DeviceSimulation.SimulatorWindow";
            titleImagePath += EditorGUIUtility.pixelsPerPoint > 1.5 ? "@2x.png" : ".png";

            titleContent = GetLocalizedTitleContent();
            titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>(titleImagePath);
            minSize = new Vector2(200, 200);
            autoRepaintOnSceneChange = true;
            clearColor = Color.black;
            playModeViewName = "Device Simulator";
            showGizmos = false;
            targetDisplay = 0;
            renderIMGUI = true;

            m_Main = new DeviceSimulatorMain(m_SimulatorState, rootVisualElement);
            s_SimulatorInstances.Add(this);

            InitPlayModeViewSwapMenu();
        }

        private void InitPlayModeViewSwapMenu()
        {
            var playModeViewTypeMenu = rootVisualElement.Q<ToolbarMenu>("playmode-view-menu");
            playModeViewTypeMenu.text = GetWindowTitle(GetType());

            var types = GetAvailableWindowTypes();
            foreach (var type in types)
            {
                var status = type.Key == GetType() ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
                playModeViewTypeMenu.menu.AppendAction(type.Value, action => SwapMainWindow(type.Key), action => status);
            }
        }

        private void OnDisable()
        {
            s_SimulatorInstances.Remove(this);
            m_Main.Dispose();
        }

        void Update()
        {
            if (m_DeviceListDirty)
            {
                m_Main.UpdateDeviceList();
                m_DeviceListDirty = false;
            }

            if (m_RestartSimulation)
            {
                m_Main.InitSimulation();
                m_RestartSimulation = false;
            }

            if (m_State == SimulationState.Disabled && GetMainPlayModeView() == this)
            {
                m_State = SimulationState.Enabled;
                m_Main.Enable();
                DeviceSimulatorCallbacks.InvokeOnDeviceChange();
            }
            else if (m_State == SimulationState.Enabled && GetMainPlayModeView() != this)
            {
                m_State = SimulationState.Disabled;
                m_Main.Disable();
                if (GetMainPlayModeView().GetType() != typeof(SimulatorWindow))
                    DeviceSimulatorCallbacks.InvokeOnDeviceChange();
            }
        }

        private void OnGUI()
        {
            if (GetMainPlayModeView() != this)
                return;

            var type = Event.current.type;
            if (type == EventType.Repaint)
            {
                targetSize = m_Main.targetSize;
                m_Main.displayTexture = RenderView(m_Main.mousePositionInUICoordinates, false);
            }
        }

        public void BeforeSerializeStates()
        {
            m_SimulatorState = m_Main.SerializeSimulatorState();
        }

#if UNITY_2020_1_OR_NEWER
        public void OnBeforeSerialize()
        {
            BeforeSerializeStates();
        }

        public void OnAfterDeserialize()
        {
        }

#else
        public new void OnBeforeSerialize()
        {
            BeforeSerializeStates();
        }

        public new void OnAfterDeserialize()
        {
        }

#endif

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload"), false, m_Main.InitSimulation);
            if (RenderDoc.IsInstalled() && !RenderDoc.IsLoaded())
            {
                menu.AddItem(EditorGUIUtility.TrTextContent(RenderDocUtil.loadRenderDocLabel), false, LoadRenderDoc);
            }
        }

        private void LoadRenderDoc()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                RenderDoc.Load();
                ShaderUtil.RecreateGfxDevice();
            }
        }

        private void OnFocus()
        {
            SetFocus(true);
        }

        public static void MarkAllDeviceListsDirty()
        {
            foreach (var simulator in s_SimulatorInstances)
            {
                simulator.m_DeviceListDirty = true;
            }
        }

        public static void RestartAllSimulators()
        {
            foreach (var simulator in s_SimulatorInstances)
            {
                simulator.m_RestartSimulation = true;
            }
        }
    }
}

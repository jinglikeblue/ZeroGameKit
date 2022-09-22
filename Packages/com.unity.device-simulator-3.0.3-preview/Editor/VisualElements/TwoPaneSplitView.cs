using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.DeviceSimulation
{
    internal class TwoPaneSplitView : VisualElement
    {
        static readonly string s_UssClassName = "unity-two-pane-split-view";
        static readonly string s_ContentContainerClassName = "unity-two-pane-split-view__content-container";
        static readonly string s_HandleDragLineClassName = "unity-two-pane-split-view__dragline";
        static readonly string s_HandleDragLineVerticalClassName = s_HandleDragLineClassName + "--vertical";
        static readonly string s_HandleDragLineHorizontalClassName = s_HandleDragLineClassName + "--horizontal";
        static readonly string s_HandleDragLineAnchorClassName = "unity-two-pane-split-view__dragline-anchor";
        static readonly string s_HandleDragLineAnchorVerticalClassName = s_HandleDragLineAnchorClassName + "--vertical";
        static readonly string s_HandleDragLineAnchorHorizontalClassName = s_HandleDragLineAnchorClassName + "--horizontal";
        static readonly string s_VerticalClassName = "unity-two-pane-split-view--vertical";
        static readonly string s_HorizontalClassName = "unity-two-pane-split-view--horizontal";

        public new class UxmlFactory : UxmlFactory<TwoPaneSplitView, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription m_FixedPaneIndex = new UxmlIntAttributeDescription { name = "fixed-pane-index", defaultValue = 0 };
            UxmlIntAttributeDescription m_FixedPaneInitialDimension = new UxmlIntAttributeDescription { name = "fixed-pane-initial-dimension", defaultValue = 100 };
            UxmlEnumAttributeDescription<TwoPaneSplitViewOrientation> m_Orientation = new UxmlEnumAttributeDescription<TwoPaneSplitViewOrientation> { name = "orientation", defaultValue = TwoPaneSplitViewOrientation.Horizontal };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var fixedPaneIndex = m_FixedPaneIndex.GetValueFromBag(bag, cc);
                var fixedPaneInitialSize = m_FixedPaneInitialDimension.GetValueFromBag(bag, cc);
                var orientation = m_Orientation.GetValueFromBag(bag, cc);

                ((TwoPaneSplitView)ve).Init(fixedPaneIndex, fixedPaneInitialSize, orientation);
            }
        }

        VisualElement m_LeftPane;
        VisualElement m_RightPane;

        VisualElement m_FixedPane;
        VisualElement m_FlexedPane;

        public VisualElement fixedPane => m_FixedPane;
        public VisualElement flexedPane => m_FlexedPane;

        VisualElement m_DragLine;
        VisualElement m_DragLineAnchor;

        bool m_CollapseMode;

        VisualElement m_Content;

        TwoPaneSplitViewOrientation m_Orientation;
        int m_FixedPaneIndex;
        float m_FixedPaneInitialDimension;

        public int fixedPaneIndex
        {
            get => m_FixedPaneIndex;
            set
            {
                if (value == m_FixedPaneIndex)
                    return;

                Init(value, m_FixedPaneInitialDimension, m_Orientation);
            }
        }

        public float fixedPaneInitialDimension
        {
            get => m_FixedPaneInitialDimension;
            set
            {
                if (value == m_FixedPaneInitialDimension)
                    return;

                Init(m_FixedPaneIndex, value, m_Orientation);
            }
        }

        public TwoPaneSplitViewOrientation orientation
        {
            get => m_Orientation;
            set
            {
                if (value == m_Orientation)
                    return;

                Init(m_FixedPaneIndex, m_FixedPaneInitialDimension, value);
            }
        }

        TwoPaneSplitViewResizer m_Resizer;

        public TwoPaneSplitView()
        {
            AddToClassList(s_UssClassName);

            m_Content = new VisualElement();
            m_Content.name = "unity-content-container";
            m_Content.AddToClassList(s_ContentContainerClassName);
            hierarchy.Add(m_Content);

            // Create drag anchor line.
            m_DragLineAnchor = new VisualElement();
            m_DragLineAnchor.name = "unity-dragline-anchor";
            m_DragLineAnchor.AddToClassList(s_HandleDragLineAnchorClassName);
            hierarchy.Add(m_DragLineAnchor);

            // Create drag
            m_DragLine = new VisualElement();
            m_DragLine.name = "unity-dragline";
            m_DragLine.AddToClassList(s_HandleDragLineClassName);
            m_DragLineAnchor.Add(m_DragLine);
        }

        public TwoPaneSplitView(
            int fixedPaneIndex,
            float fixedPaneStartDimension,
            TwoPaneSplitViewOrientation orientation) : this()
        {
            Init(fixedPaneIndex, fixedPaneStartDimension, orientation);
        }

        public void CollapseChild(int index)
        {
            if (m_LeftPane == null)
                return;

            m_DragLine.style.display = DisplayStyle.None;
            m_DragLineAnchor.style.display = DisplayStyle.None;
            if (index == 0)
            {
                m_RightPane.style.width = StyleKeyword.Initial;
                m_RightPane.style.height = StyleKeyword.Initial;
                m_RightPane.style.flexGrow = 1;
                m_LeftPane.style.display = DisplayStyle.None;
            }
            else
            {
                m_LeftPane.style.width = StyleKeyword.Initial;
                m_LeftPane.style.height = StyleKeyword.Initial;
                m_LeftPane.style.flexGrow = 1;
                m_RightPane.style.display = DisplayStyle.None;
            }

            m_CollapseMode = true;
        }

        public void UnCollapse()
        {
            if (m_LeftPane == null)
                return;

            m_LeftPane.style.display = DisplayStyle.Flex;
            m_RightPane.style.display = DisplayStyle.Flex;

            m_DragLine.style.display = DisplayStyle.Flex;
            m_DragLineAnchor.style.display = DisplayStyle.Flex;

            m_LeftPane.style.flexGrow = 0;
            m_RightPane.style.flexGrow = 0;
            m_CollapseMode = false;

            Init(m_FixedPaneIndex, m_FixedPaneInitialDimension, m_Orientation);
        }

        internal void Init(int fixedPaneIndex, float fixedPaneInitialDimension, TwoPaneSplitViewOrientation orientation)
        {
            m_Orientation = orientation;
            m_FixedPaneIndex = fixedPaneIndex;
            m_FixedPaneInitialDimension = fixedPaneInitialDimension;

            m_Content.RemoveFromClassList(s_HorizontalClassName);
            m_Content.RemoveFromClassList(s_VerticalClassName);
            if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
                m_Content.AddToClassList(s_HorizontalClassName);
            else
                m_Content.AddToClassList(s_VerticalClassName);

            // Create drag anchor line.
            m_DragLineAnchor.RemoveFromClassList(s_HandleDragLineAnchorHorizontalClassName);
            m_DragLineAnchor.RemoveFromClassList(s_HandleDragLineAnchorVerticalClassName);
            if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
                m_DragLineAnchor.AddToClassList(s_HandleDragLineAnchorHorizontalClassName);
            else
                m_DragLineAnchor.AddToClassList(s_HandleDragLineAnchorVerticalClassName);

            // Create drag
            m_DragLine.RemoveFromClassList(s_HandleDragLineHorizontalClassName);
            m_DragLine.RemoveFromClassList(s_HandleDragLineVerticalClassName);
            if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
                m_DragLine.AddToClassList(s_HandleDragLineHorizontalClassName);
            else
                m_DragLine.AddToClassList(s_HandleDragLineVerticalClassName);

            if (m_Resizer != null)
            {
                m_DragLineAnchor.RemoveManipulator(m_Resizer);
                m_Resizer = null;
            }

            if (m_Content.childCount != 2)
                RegisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            else
                PostDisplaySetup();
        }

        void OnPostDisplaySetup(GeometryChangedEvent evt)
        {
            if (m_Content.childCount != 2)
            {
                Debug.LogError("TwoPaneSplitView needs exactly 2 chilren.");
                return;
            }

            PostDisplaySetup();

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            RegisterCallback<GeometryChangedEvent>(OnSizeChange);
        }

        void PostDisplaySetup()
        {
            if (m_Content.childCount != 2)
            {
                Debug.LogError("TwoPaneSplitView needs exactly 2 children.");
                return;
            }

            m_LeftPane = m_Content[0];
            if (m_FixedPaneIndex == 0)
                m_FixedPane = m_LeftPane;
            else
                m_FlexedPane = m_LeftPane;

            m_RightPane = m_Content[1];
            if (m_FixedPaneIndex == 1)
                m_FixedPane = m_RightPane;
            else
                m_FlexedPane = m_RightPane;

            m_FixedPane.style.flexBasis = StyleKeyword.Null;
            m_FixedPane.style.flexShrink = StyleKeyword.Null;
            m_FixedPane.style.flexGrow = StyleKeyword.Null;
            m_FlexedPane.style.flexGrow = StyleKeyword.Null;
            m_FlexedPane.style.flexShrink = StyleKeyword.Null;
            m_FlexedPane.style.flexBasis = StyleKeyword.Null;

            m_FixedPane.style.width = StyleKeyword.Null;
            m_FixedPane.style.height = StyleKeyword.Null;
            m_FlexedPane.style.width = StyleKeyword.Null;
            m_FlexedPane.style.height = StyleKeyword.Null;

            if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
            {
                m_FixedPane.style.width = m_FixedPaneInitialDimension;
                m_FixedPane.style.height = StyleKeyword.Null;
            }
            else
            {
                m_FixedPane.style.width = StyleKeyword.Null;
                m_FixedPane.style.height = m_FixedPaneInitialDimension;
            }

            m_FixedPane.style.flexShrink = 0;
            m_FixedPane.style.flexGrow = 0;
            m_FlexedPane.style.flexGrow = 1;
            m_FlexedPane.style.flexShrink = 0;
            m_FlexedPane.style.flexBasis = 0;

            if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
            {
                if (m_FixedPaneIndex == 0)
                    m_DragLineAnchor.style.left = m_FixedPaneInitialDimension;
                else
                    m_DragLineAnchor.style.left = this.resolvedStyle.width - m_FixedPaneInitialDimension;
            }
            else
            {
                if (m_FixedPaneIndex == 0)
                    m_DragLineAnchor.style.top = m_FixedPaneInitialDimension;
                else
                    m_DragLineAnchor.style.top = this.resolvedStyle.height - m_FixedPaneInitialDimension;
            }

            int direction = 1;
            if (m_FixedPaneIndex == 0)
                direction = 1;
            else
                direction = -1;

            if (m_FixedPaneIndex == 0)
                m_Resizer = new TwoPaneSplitViewResizer(this, direction, m_Orientation);
            else
                m_Resizer = new TwoPaneSplitViewResizer(this, direction, m_Orientation);

            m_DragLineAnchor.AddManipulator(m_Resizer);

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            RegisterCallback<GeometryChangedEvent>(OnSizeChange);
        }

        void OnSizeChange(GeometryChangedEvent evt)
        {
            OnSizeChange();
        }

        void OnSizeChange()
        {
            if (m_CollapseMode)
                return;

            var maxLength = this.resolvedStyle.width;
            var dragLinePos = m_DragLineAnchor.resolvedStyle.left;
            var activeElementPos = m_FixedPane.resolvedStyle.left;
            if (m_Orientation == TwoPaneSplitViewOrientation.Vertical)
            {
                maxLength = this.resolvedStyle.height;
                dragLinePos = m_DragLineAnchor.resolvedStyle.top;
                activeElementPos = m_FixedPane.resolvedStyle.top;
            }

            if (m_FixedPaneIndex == 0 && dragLinePos > maxLength)
            {
                var delta = maxLength - dragLinePos;
                m_Resizer.ApplyDelta(delta);
            }
            else if (m_FixedPaneIndex == 1)
            {
                if (activeElementPos < 0)
                {
                    var delta = -dragLinePos;
                    m_Resizer.ApplyDelta(delta);
                }
                else
                {
                    if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
                        m_DragLineAnchor.style.left = activeElementPos;
                    else
                        m_DragLineAnchor.style.top = activeElementPos;
                }
            }
        }

        public override VisualElement contentContainer
        {
            get { return m_Content; }
        }
    }

    internal enum TwoPaneSplitViewOrientation
    {
        Horizontal,
        Vertical
    }

    internal class TwoPaneSplitViewResizer : MouseManipulator
    {
        Vector2 m_Start;
        protected bool m_Active;
        TwoPaneSplitView m_SplitView;

        int m_Direction;
        TwoPaneSplitViewOrientation m_Orientation;

        VisualElement fixedPane => m_SplitView.fixedPane;
        VisualElement flexedPane => m_SplitView.flexedPane;

        float fixedPaneMinDimension
        {
            get
            {
                if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
                    return fixedPane.resolvedStyle.minWidth.value;
                else
                    return fixedPane.resolvedStyle.minHeight.value;
            }
        }

        float flexedPaneMinDimension
        {
            get
            {
                if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
                    return flexedPane.resolvedStyle.minWidth.value;
                else
                    return flexedPane.resolvedStyle.minHeight.value;
            }
        }

        public TwoPaneSplitViewResizer(TwoPaneSplitView splitView, int dir, TwoPaneSplitViewOrientation orientation)
        {
            m_Orientation = orientation;
            m_SplitView = splitView;
            m_Direction = dir;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            m_Active = false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        public void ApplyDelta(float delta)
        {
            float oldDimension = m_Orientation == TwoPaneSplitViewOrientation.Horizontal
                ? fixedPane.resolvedStyle.width
                : fixedPane.resolvedStyle.height;
            float newDimension = oldDimension + delta;

            if (newDimension < oldDimension && newDimension < fixedPaneMinDimension)
                newDimension = fixedPaneMinDimension;

            float maxDimension = m_Orientation == TwoPaneSplitViewOrientation.Horizontal
                ? m_SplitView.resolvedStyle.width
                : m_SplitView.resolvedStyle.height;
            maxDimension -= flexedPaneMinDimension;
            if (newDimension > oldDimension && newDimension > maxDimension)
                newDimension = maxDimension;

            if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
            {
                fixedPane.style.width = newDimension;
                if (m_SplitView.fixedPaneIndex == 0)
                    target.style.left = newDimension;
                else
                    target.style.left = m_SplitView.resolvedStyle.width - newDimension;
            }
            else
            {
                fixedPane.style.height = newDimension;
                if (m_SplitView.fixedPaneIndex == 0)
                    target.style.top = newDimension;
                else
                    target.style.top = m_SplitView.resolvedStyle.height - newDimension;
            }
        }

        protected void OnMouseDown(MouseDownEvent e)
        {
            if (m_Active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (CanStartManipulation(e))
            {
                m_Start = e.localMousePosition;

                m_Active = true;
                target.CaptureMouse();
                e.StopPropagation();
            }
        }

        protected void OnMouseMove(MouseMoveEvent e)
        {
            if (!m_Active || !target.HasMouseCapture())
                return;

            Vector2 diff = e.localMousePosition - m_Start;
            float mouseDiff = diff.x;
            if (m_Orientation == TwoPaneSplitViewOrientation.Vertical)
                mouseDiff = diff.y;

            float delta = m_Direction * mouseDiff;

            ApplyDelta(delta);

            e.StopPropagation();
        }

        protected void OnMouseUp(MouseUpEvent e)
        {
            if (!m_Active || !target.HasMouseCapture() || !CanStopManipulation(e))
                return;

            m_Active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }
    }
}

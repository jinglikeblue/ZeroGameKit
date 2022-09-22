# Device Simulator plugins

Device Simulator allows you to use plugins to extend its functionality and change the UI of the Control Panel in the Device Simulator view.

## Creating a plugin

To create a Device simulator plugin extend the [DeviceSimulatorPlugin](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/DeviceSimulation.DeviceSimulatorPlugin.html) class. 

* Override the [title](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/DeviceSimulation.DeviceSimulatorPlugin-title.html) property to return a non empty string
* Override the [OnCreateUI](https://docs.unity3d.com/2021.1/Documentation/ScriptReference/DeviceSimulation.DeviceSimulatorPlugin.OnCreateUI.html) method to return a [VisualElement](https://docs.unity3d.com/ScriptReference/UIElements.VisualElement.html) containing the UI.

If these conditions are not met, the pugin will be instantiated, but its UI will not be visible in the simulator window.

### Example

```csharp
public class TouchInfoPlugin : DeviceSimulatorPlugin
{
    public override string title => "Touch Info";
    private Label m_TouchCountLabel;
    private Label m_LastTouchEvent;
    private Button m_ResetCountButton;

    [SerializeField]
    private int m_TouchCount = 0;

    public override void OnCreate()
    {
        deviceSimulator.touchScreenInput += touchEvent =>
        {
            m_TouchCount += 1;
            UpdateTouchCounterText();
            m_LastTouchEvent.text = $"Last touch event: {touchEvent.phase.ToString()}";
        };
    }

    public override VisualElement OnCreateUI()
    {
        VisualElement root = new VisualElement();

        m_LastTouchEvent = new Label("Last touch event: None");

        m_TouchCountLabel = new Label();
        UpdateTouchCounterText();

        m_ResetCountButton = new Button {text = "Reset Count" };
        m_ResetCountButton.clicked += () =>
        {
            m_TouchCount = 0;
            UpdateTouchCounterText();
        };

        root.Add(m_LastTouchEvent);
        root.Add(m_TouchCountLabel);
        root.Add(m_ResetCountButton);

        return root;
    }

    private void UpdateTouchCounterText()
    {
        if (m_TouchCount > 0)
            m_TouchCountLabel.text = $"Touches recorded: {m_TouchCount}";
        else
            m_TouchCountLabel.text = "No taps recorded";
    }
}
```

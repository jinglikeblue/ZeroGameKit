using System;
using UnityEngine;

namespace UnityEditor.DeviceSimulation
{
    /// <summary>
    /// Phase of a touch event.
    /// </summary>
    public enum TouchPhase
    {
        /// <summary>A touch has begun. Only the first touch event in any given touch will have this phase.</summary>
        Began,
        /// <summary>A touch has changed position.</summary>
        Moved,
        /// <summary>A touch has ended. Only the last touch event in a given touch will have this phase.</summary>
        Ended,
        /// <summary>A touch has ended in a way other than through user interaction.</summary>
        Canceled,
        /// <summary>A touch has not moved.</summary>
        Stationary
    }

    /// <summary>
    /// Representation of a single touch event coming from a Device Simulator. Subscribe to DeviceSimulator.touchScreenInput to receive these events.
    /// </summary>
    public struct TouchEvent
    {
        internal TouchEvent(int touchId, Vector2 position, TouchPhase phase)
        {
            this.touchId = touchId;
            this.position = position;
            this.phase = phase;
        }

        /// <summary>
        /// The unique identifier for the touch. Unity reuses identifiers after the touch ends.
        /// </summary>
        /// <remarks>Test</remarks>
        /// <value>ID of the tocuh.</value>
        public int touchId { get; }

        /// <summary>
        /// On-screen position of the touch event. The zero point is at the bottom-left corner of the screen in pixel coordinates.
        /// </summary>
        /// <remarks>Test</remarks>
        /// <value>Position of the touch.</value>
        public Vector2 position { get; }

        /// <summary>
        /// Phase of the touch event.
        /// </summary>
        /// <remarks>Test</remarks>
        /// <value>Phase of a touch.</value>
        public TouchPhase phase { get; }
    }

    /// <summary>
    /// Class for interacting with a Device Simulator window from a script.
    /// You can get an instance of this class by extending DeviceSimulatorPlugin.
    /// </summary>
    public class DeviceSimulator
    {
        internal DeviceSimulator()
        {
        }

        internal ApplicationSimulation applicationSimulation;

        /// <summary>
        /// Event invoked when the screen of the simulated device is clicked.
        /// </summary>
        /// <value>Event users can register for screen touches.</value>
        public event Action<TouchEvent> touchScreenInput;

        internal void OnTouchScreenInput(TouchEvent touchEvent)
        {
            var handlers = touchScreenInput?.GetInvocationList();
            if (handlers == null)
                return;

            foreach (Action<TouchEvent> handler in handlers)
            {
                try
                {
                    handler.Invoke(touchEvent);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}

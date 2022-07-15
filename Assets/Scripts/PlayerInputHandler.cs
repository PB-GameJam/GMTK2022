// Author:  Joseph Crump
// Date:    07/03/22

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Component that handles events dispatched by the <see cref="PlayerInput"/>.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public abstract class VirtualInputValue<TValue> where TValue : struct
    {
        /// <summary>
        /// Current input value of the axis.
        /// </summary>
        public TValue Value { get; private set; }

        public Action<InputAction.CallbackContext> InputCallback => OnInputChanged;

        private void OnInputChanged(InputAction.CallbackContext context)
        {
            Value = context.ReadValue<TValue>();
        }
    }

    public class VirtualVector : VirtualInputValue<Vector2>
    {
        /// <summary>
        /// X-axis of the vector Value.
        /// </summary>
        public float Horizontal => Value.x;

        /// <summary>
        /// Y-axis of the vector value.
        /// </summary>
        public float Vertical => Value.y;
    }

    public class VirtualAxis : VirtualInputValue<float> { }

    public class VirtualButton
    {
        /// <summary>
        /// Current pressed state of the button.
        /// </summary>
        public bool IsDown { get; private set; }

        /// <summary>
        /// Event raised when the button is first pressed.
        /// </summary>
        public event Action Pressed;

        public Action<InputAction.CallbackContext> InputCallback => OnInputUsed;

        private void OnInputUsed(InputAction.CallbackContext context)
        {
            IsDown = context.ReadValueAsButton();

            if (context.performed)
                Pressed?.Invoke();
        }
    }

    private class CallbackInfo
    {
        public string Action;
        public Action<InputAction.CallbackContext> Callback;
    }

    private PlayerInput _input;

    private readonly VirtualVector _move = new VirtualVector();
    private readonly VirtualVector _look = new VirtualVector();
    private readonly VirtualAxis _altitude = new VirtualAxis();
    private readonly VirtualButton _jump = new VirtualButton();
    private readonly VirtualButton _toggleMoveState = new VirtualButton();

    private readonly List<CallbackInfo> registeredCallbacks = new List<CallbackInfo>();

    /// <summary>
    /// The input axis used to move the player character.
    /// </summary>
    public VirtualVector Move => _move;

    /// <summary>
    /// The input axis used to steer the camera.
    /// </summary>
    public VirtualVector Look => _look;

    /// <summary>
    /// The input axis used to control altitude in flight mode.
    /// </summary>
    public VirtualAxis Altitude => _altitude;

    /// <summary>
    /// The input button used to jump.
    /// </summary>
    public VirtualButton Jump => _jump;

    /// <summary>
    /// The input used to toggle between default movement and flight mode.
    /// </summary>
    public VirtualButton ToggleMoveState => _toggleMoveState;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        RegisterCallback("Move", Move.InputCallback);
        RegisterCallback("Look", Look.InputCallback);
        RegisterCallback("Jump", Jump.InputCallback);
        RegisterCallback("Altitude", Altitude.InputCallback);
        RegisterCallback("ToggleMoveState", ToggleMoveState.InputCallback);
    }

    private void OnDisable()
    {
        DeregisterAllCallbacks();
    }

    private void RegisterCallback(string action, Action<InputAction.CallbackContext> callback)
    {
        _input.actions[action].started += callback;
        _input.actions[action].performed += callback;
        _input.actions[action].canceled += callback;

        registeredCallbacks.Add(new CallbackInfo { Action = action, Callback = callback });
    }

    private void DeregisterAllCallbacks()
    {
        registeredCallbacks.ForEach(item => DeregisterCallback(item.Action, item.Callback));
    }

    private void DeregisterCallback(string action, Action<InputAction.CallbackContext> callback)
    {
        _input.actions[action].started -= callback;
        _input.actions[action].performed -= callback;
        _input.actions[action].canceled -= callback;
    }
}

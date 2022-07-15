// Author:  Joseph Crump
// Date:    07/03/22

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behavior that processes the player's inputs and moves the player avatar.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    public enum MoveMode
    {
        Default,
        Flight
    }

    [Header("Settings")]
    [SerializeField]
    [Tooltip("The initial move state for the controller.")]
    private MoveMode _moveMode = MoveMode.Default;

    [SerializeField]
    [Tooltip("How quickly the player moves in any direction while in default mode.")]
    private float _runSpeed = 5f;

    [SerializeField]
    [Tooltip("How quickly the player moves while in flight mode.")]
    private float _flightSpeed = 15f;

    [SerializeField]
    private float _minCameraPitch = -80f, _maxCameraPitch = 80f;

    [SerializeField, Range(0f, 1f)]
    private float _pitchInterpolant = 0.2f;

    [SerializeField, Min(0f)]
    private float _pitchSensitivity = 0.5f, _yawSensitivity = 0.8f;

    [SerializeField]
    private bool _invertYaw = false, _invertPitch = false;

    [SerializeField]
    private float _jumpforce = 8f;

    [Header("References")]
    [SerializeField]
    private PlayerInputHandler _input;

    [SerializeField]
    private Transform _cameraTarget;

    [SerializeField]
    private GroundDetector _groundDetector;

    [SerializeField]
    private Collider _playerCollider;

    private Rigidbody _rigidbody;

    private float _targetPitch = 0f;
    private float TargetPitch
    {
        get => _targetPitch;
        set
        {
            _targetPitch = Mathf.Clamp(value, _minCameraPitch, _maxCameraPitch);
        }
    }

    private Action _moveCommand = null;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        SetMoveMove(_moveMode);
    }

    private void OnValidate()
    {
        if (_input == null)
            _input = GetComponentInChildren<PlayerInputHandler>();

        if (_groundDetector == null)
            _groundDetector = GetComponentInChildren<GroundDetector>();

        if (_playerCollider == null)
            _playerCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (Application.isFocused == false)
            return;

        _moveCommand();
        Look();
    }

    private void OnEnable()
    {
        ShowMouseCursor(false);

        _input.Jump.Pressed += Jump;
        _input.ToggleMoveState.Pressed += ToggleMoveMode;
    }

    private void OnDisable()
    {
        ShowMouseCursor(true);

        _input.Jump.Pressed -= Jump;
        _input.ToggleMoveState.Pressed -= ToggleMoveMode;
    }

    private void OnApplicationFocus(bool focus)
    {
        ShowMouseCursor(!focus);
    }

    /// <summary>
    /// Change the movement mode of the controller between default and 
    /// flight mode. This changes the way movement, jumping, and gravity
    /// is performed.
    /// </summary>
    public void SetMoveMove(MoveMode mode)
    {
        _moveMode = mode;

        bool isFlightMode = (_moveMode == MoveMode.Flight);
        _rigidbody.useGravity = !isFlightMode;
        _moveCommand = (isFlightMode) ? FlightMove : DefaultMove;
        _playerCollider.isTrigger = isFlightMode;
        _rigidbody.velocity = Vector3.zero; // reset velocity when changing
    }

    private void DefaultMove()
    {
        float strafe = _input.Move.Horizontal;
        float forward = _input.Move.Vertical;

        var direction = new Vector3(strafe, 0f, forward).normalized;
        direction = transform.TransformVector(direction) * _runSpeed;

        // preserve Y velocity
        direction.y = _rigidbody.velocity.y;
        _rigidbody.velocity = direction;
    }

    private void FlightMove()
    {
        Vector3 strafe = _cameraTarget.right * _input.Move.Horizontal;
        Vector3 forward = _cameraTarget.forward * _input.Move.Vertical;
        Vector3 altitude = new Vector3(0f, _input.Altitude.Value, 0f);

        var direction = (strafe + forward + altitude).normalized;

        _rigidbody.velocity = direction * _flightSpeed;
    }

    private void Look()
    {
        float yawDelta = _input.Look.Horizontal * _yawSensitivity;
        yawDelta = (_invertYaw) ? -yawDelta : yawDelta;
        transform.Rotate(0f, yawDelta, 0f);

        float pitchDelta = _input.Look.Vertical * _pitchSensitivity;
        pitchDelta = (_invertPitch) ? -pitchDelta : pitchDelta;

        TargetPitch += pitchDelta;
        var currentRotation = _cameraTarget.localRotation;
        var newRotation = Quaternion.Euler(TargetPitch, currentRotation.y, currentRotation.z);
        _cameraTarget.localRotation = Quaternion.Slerp(currentRotation, newRotation, _pitchInterpolant);
    }

    private void Jump()
    {
        if (_moveMode == MoveMode.Flight)
            return;

        if (!_groundDetector.IsGrounded)
            return;

        _rigidbody.AddForce(transform.up * _jumpforce, ForceMode.Impulse);
    }

    private void ToggleMoveMode()
    {
        if (_moveMode == MoveMode.Default)
            SetMoveMove(MoveMode.Flight);
        else if (_moveMode == MoveMode.Flight)
            SetMoveMove(MoveMode.Default);
    }

    private void ShowMouseCursor(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }
}

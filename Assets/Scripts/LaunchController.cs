using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchController : MonoBehaviour
{
    [SerializeField] private float MinLaunchSpeed;
    [SerializeField] private float MaxLaunchSpeed;
    [SerializeField] private float MaxJumpHoldTime;

    private float CurrentHoldTime;

    private GroundDetector GD => Registry.Lookup<GroundDetector>();
    private PlayerInputHandler Input => Registry.Lookup<PlayerInputHandler>();
    private Rigidbody RBD
    {
        get
        {
            if (_RBD == null)
                _RBD = GetComponentInChildren<Rigidbody>();

            return _RBD;
        }
    }

    private Rigidbody _RBD;

    private Camera MainCam => Camera.main;

    private void OnEnable()
    {
        Input.Jump.Released += LaunchAttempt;
    }

    private void OnDisable()
    {
        Input.Jump.Released -= LaunchAttempt;
    }

    public void Update()
    {
        if (GD.IsGrounded == false)
            return;

        if (Input.Jump.IsDown == true)
            ChargeJump();
    }

    public void ChargeJump()
    {
        CurrentHoldTime += Time.deltaTime;

        if (CurrentHoldTime > MaxJumpHoldTime)
        {
            LaunchAttempt();
        }
    }

    public void LaunchAttempt()
    {
        if (GD.IsGrounded == false)
            return;

        
        float interpolant = CurrentHoldTime / MaxJumpHoldTime;
        float launchSpeed = Mathf.Lerp(MinLaunchSpeed, MaxLaunchSpeed, interpolant);

        RBD.velocity = MainCam.transform.forward * launchSpeed;

        StopCharging();
    }

    public void StopCharging()
    {
        CurrentHoldTime = 0F;
    }
}

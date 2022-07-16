using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchController : MonoBehaviour
{
    [SerializeField] private float MaxLaunchSpeed;
    [SerializeField] private float VerticalSpeed;
    [SerializeField] private AudioClip LaunchClip;

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
        Input.Jump.Pressed += LaunchAttempt;
    }

    private void OnDisable()
    {
        Input.Jump.Pressed -= LaunchAttempt;
    }

    public void LaunchAttempt()
    {
        if (GD.IsGrounded == false)
            return;

        RBD.velocity = MainCam.transform.forward * MaxLaunchSpeed;
        RBD.velocity += Vector3.up * VerticalSpeed;

        AudioSource.PlayClipAtPoint(LaunchClip, Camera.main.transform.position);
    }
}

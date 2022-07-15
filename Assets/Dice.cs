using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Dice : MonoBehaviour
{
    [SerializeField] private float MinVerticalSpeed;
    [SerializeField] private float MaxVerticalSpeed;
    [SerializeField] private float LateralSpeed;
    [SerializeField] private float MinTorque;
    [SerializeField] private float MaxTorque;
    [SerializeField] private float GravitySpeed;
    [SerializeField] private float DiceReadVelocityThreshold;

    [SerializeField] private GameObject OnHitVFX;
    [SerializeField] private AudioSource HitSource;

    public event Action DiceSettled;

    private CollisionTracker CollisionTracker => Registry.Lookup<CollisionTracker>();
    private CinemachineVirtualCamera FollowCam
    {
        get
        {
            if (_FollowCam == null)
                _FollowCam = CollisionTracker.GetComponent<CinemachineVirtualCamera>();

            return _FollowCam;
        }
    }

    private CinemachineVirtualCamera _FollowCam;


    private bool HasBeenHit
    {
        get
        {
            return _HasBeenHit;
        }

        set
        {
            _HasBeenHit = value;

            if (value == true)
            {
                FollowCam.Follow = transform;
                FollowCam.Priority = 100;
                RBD.isKinematic = false;
            }
            else if (value == false)
            {
                FollowCam.Priority = 0;
                RBD.isKinematic = true;
            }
        }
    }

    private bool _HasBeenHit;

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

    private void FixedUpdate()
    {
        RBD.velocity += Vector3.down * GravitySpeed;

        if (HasBeenHit == false)
            return;

        if (RBD.velocity.magnitude < DiceReadVelocityThreshold)
        {
            HasBeenHit = false;
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (HasBeenHit == true)
            return;

        HasBeenHit = true;
        
        Vector3 speedVector = UnityEngine.Random.insideUnitSphere;
        float lateralSpeed = UnityEngine.Random.Range(-LateralSpeed, LateralSpeed);
        float verticalSpeed = UnityEngine.Random.Range(MinVerticalSpeed, MaxVerticalSpeed);
        speedVector = new Vector3(lateralSpeed, verticalSpeed, lateralSpeed);

        Vector3 torque = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(MinTorque, MaxTorque);

        RBD.AddTorque(torque, ForceMode.Impulse);

        RBD.velocity = speedVector;
    }
}

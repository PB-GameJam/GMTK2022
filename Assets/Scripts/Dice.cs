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
    [SerializeField] private float DiceHoldTime;

    [SerializeField] private GameObject OnHitVFX;
    [SerializeField] private AudioSource HitSource;

    public event Action DiceSettled;

    private CollisionTracker CollisionTracker => Registry.Lookup<CollisionTracker>();
    private CinemachineVirtualCamera FollowCam => Registry.Lookup<CinemachineVirtualCamera>(CollisionTracker);
    private Rigidbody RBD => Registry.Lookup<Rigidbody>(this);

    private float PreviousSpeed;
    private float SpeedDifferential;
    private float HoldTimer;

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
                FindObjectOfType<GameManager>().PauseTimer();
                FollowCam.Follow = transform;
                FollowCam.Priority = 100;
                RBD.isKinematic = false;
            }
            else if (value == false)
            {
                FindObjectOfType<GameManager>().ResumeTimer();
                FollowCam.Priority = 0;
                HoldTimer = 0F;
                RBD.isKinematic = true;
            }
        }
    }

    private bool _HasBeenHit;

    private void FixedUpdate()
    {
        if (HasBeenHit == false)
            return;

        RBD.velocity += Vector3.down * GravitySpeed;

        SpeedDifferential = RBD.velocity.magnitude - PreviousSpeed;
        PreviousSpeed = RBD.velocity.magnitude;

        if (SpeedDifferential < DiceReadVelocityThreshold)
        {
            HoldTimer += Time.deltaTime;

            if (HoldTimer > DiceHoldTime)
            {
                HasBeenHit = false;
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
            return;

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

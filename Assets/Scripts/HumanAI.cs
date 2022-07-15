using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class HumanAI : MonoBehaviour
{
    [SerializeField] private Collider TargetDetectTrigger;
    [SerializeField] private string TargetTag = "Player";
    [SerializeField] private bool GoTowardTarget = false;

    [SerializeField] private float WalkForce = 5f;
    [SerializeField] private float JumpForce = 5f;

    [SerializeField] private float TurnSpeed = 5f;

    [SerializeField] private float MinMoveTimer = 0.5f;
    [SerializeField] private float MaxMoveTimer = 1.5f;
    [SerializeField] private float MinJumpTimer = 1f;
    [SerializeField] private float MaxJumpTimer = 3f;

    private float MoveTimer;
    private float MoveTimerTarget;
    private float JumpTimer;
    private float JumpTimerTarget;

    private Vector3 WalkDir;

    private Rigidbody RB;

    private Transform TargetTrans;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        TargetDetectTrigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TargetTag))
            TargetTrans = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TargetTag))
            TargetTrans = null;
    }

    // Update is called once per frame
    void Update()
    {
        //movement direction setting
        if (TargetTrans != null)
        {
            WalkDir = TargetTrans.position - transform.position;
            WalkDir.y = 0;

            //if set to move away from the target, invert the direction
            if (!GoTowardTarget)
                WalkDir.Scale(new Vector3(-1f, 0f, -1f));
        }
        else if (MoveTimer >= MoveTimerTarget)
            SetRandWalkDir();
        else
            MoveTimer += Time.deltaTime;

        //jump checking
        if (JumpTimer >= JumpTimerTarget)
            Jump();
        else
            JumpTimer += Time.deltaTime;

        //turn character toward the walk direction
        Vector3 lookStep = Vector3.RotateTowards(transform.forward, WalkDir, TurnSpeed, 0f);

        transform.rotation = Quaternion.LookRotation(lookStep);
    }

    private void FixedUpdate()
    {
        //walk in the walkdir
        RB.AddForce(WalkDir.normalized * WalkForce, ForceMode.Force);
    }

    private void SetRandWalkDir()
    {
        //reset timer
        MoveTimer = 0f;
        MoveTimerTarget = Random.Range(MinMoveTimer, MaxMoveTimer);

        WalkDir = Random.onUnitSphere;
        WalkDir.y = 0f;
    }

    private void Jump()
    {
        //reset timer
        JumpTimer = 0f;
        JumpTimerTarget = Random.Range(MinJumpTimer, MaxJumpTimer);

        RB.AddForce((Vector3.up * JumpForce), ForceMode.Impulse);
    }
}
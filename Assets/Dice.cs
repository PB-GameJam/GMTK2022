using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] private float MinVerticalSpeed;
    [SerializeField] private float MaxVerticalSpeed;
    [SerializeField] private float LateralSpeed;
    [SerializeField] private float MinTorque;
    [SerializeField] private float MaxTorque;

    private bool HasBeenHit = false;

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

    public void OnCollisionEnter(Collision collision)
    {
        if (HasBeenHit == true)
            return;

        HasBeenHit = true;
        RBD.isKinematic = false;

        Vector3 speedVector = Random.insideUnitSphere;
        float lateralSpeed = Random.Range(-LateralSpeed, LateralSpeed);
        float verticalSpeed = Random.Range(MinVerticalSpeed, MaxVerticalSpeed);
        speedVector = new Vector3(lateralSpeed, verticalSpeed, lateralSpeed);

        Vector3 torque = Random.insideUnitSphere * Random.Range(MinTorque, MaxTorque);

        RBD.AddTorque(torque);

        RBD.velocity = speedVector;
    }
}

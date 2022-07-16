using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalGravity : MonoBehaviour
{
    [SerializeField] private float GravitySpeed;

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

    public void FixedUpdate()
    {
        RBD.velocity += Vector3.down * GravitySpeed;
    }
}

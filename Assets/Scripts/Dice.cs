using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Dice : MonoBehaviour
{
    [SerializeField] private float SelfKnockback;
    [SerializeField] private float MinTorque;
    [SerializeField] private float MaxTorque;
    [SerializeField] private float GravitySpeed;
    [SerializeField] private float UpwardForce;

    [SerializeField] private GameObject OnHitVFX;
    [SerializeField] private AudioSource HitSource;

    [Range(0F, 1F)] [SerializeField] private float DampeningFactor;

    public event Action DiceSettled;

    private CollisionTracker CollisionTracker => Registry.Lookup<CollisionTracker>();
    private Rigidbody RBD => Registry.Lookup<Rigidbody>(this);

    private bool _HasBeenHit;

    private void FixedUpdate()
    {
        RBD.velocity += Vector3.down * GravitySpeed;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
            return;

        collision.collider.GetComponent<Rigidbody>().velocity *= DampeningFactor;
        Vector3 knockbackForce = (transform.position - collision.collider.transform.position).normalized * SelfKnockback;
        knockbackForce += Vector3.up * UpwardForce;
        Vector3 torque = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(MinTorque, MaxTorque);
        RBD.AddForce(knockbackForce, ForceMode.Impulse);
        RBD.AddTorque(torque, ForceMode.Impulse);

    }
}

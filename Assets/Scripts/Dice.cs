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
    [SerializeField] private float MovementEpsilon = 1F;
    [SerializeField] private float DiceSideCheckTime = 4F;
    

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

        StartCoroutine(CheckDiceSideCR(collision.collider.gameObject));
    }

    IEnumerator CheckDiceSideCR(GameObject _player)
    {
        // Add some UX visuals for the dice

        yield return new WaitForSeconds(DiceSideCheckTime);

        CheckDiceSide(_player);
    }

    public void CheckDiceSide(GameObject _player)
    {
        Debug.Log("Checking Dice Side for Powerup for " + _player);
    }


}

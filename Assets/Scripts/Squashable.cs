using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class Squashable : MonoBehaviour
{
    [Tooltip("DisableWhenSquashed")]
    [SerializeField] private MonoBehaviour[] DisableBehaviors;

    [SerializeField] private string SquasherTag = "Player";

    [SerializeField] private Vector3 SquashScaleFactor = new Vector3(3f, .1f, 3f);

    [SerializeField] private float SquashSpeed = 3f;

    [SerializeField] List<AudioClip> SquashSounds = new List<AudioClip>();

    private Collider[] DisableColliders;
    private Rigidbody RB;

    Vector3 OriginalScale;
    Vector3 TargetScale;
    float Timer = 0f;
    bool Squashing = false;

    private void Awake()
    {
        OriginalScale = transform.localScale;

        DisableColliders = GetComponentsInChildren<Collider>();

        RB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag(SquasherTag))
        {
            foreach (Collider col in DisableColliders)
            {
                col.enabled = false;
            }

            foreach (MonoBehaviour behaviour in DisableBehaviors)
            {
                behaviour.enabled = false;
            }

            if(RB != null)
            {
                RB.useGravity = false;
                RB.velocity = Vector3.zero;
                RB.freezeRotation = true;
            }

            // Play squash sounds
            AudioSource.PlayClipAtPoint(SquashSounds[Random.Range(0, SquashSounds.Count - 1)], Camera.main.transform.position);

            //disable all other components and face the object upright
            transform.up = Vector3.up;

            //set the target scale
            TargetScale = OriginalScale;
            TargetScale.Scale(SquashScaleFactor);

            Squashing = true;

            //ADD SOUND HERE
        }
    }

    private void Update()
    {
        if(Squashing)
        {
            Timer += Time.deltaTime * SquashSpeed;

            transform.localScale = Vector3.Lerp(OriginalScale, TargetScale, Timer);

            if (transform.localScale.y <= TargetScale.y)
            {
                Squashing = false;
                this.enabled = false;
            }
        }
    }
}

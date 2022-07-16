using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Destruction : MonoBehaviour
{
    [Header("Score Variable")]
    [SerializeField] private int Score = 5;

    [Header("Squish Variables")]
    [SerializeField] private bool canSquish = true;
    [SerializeField] private float squishDuration = 5;
    [SerializeField] private float squishSize = 0.1f;
    [SerializeField] private Ease EaseType;

    [Header("When Destroyed")]
    [SerializeField] AudioClip[] ExplosionSFX;
    [SerializeField] private UnityEvent ObjectHitByPlayer;
    private bool isDestroyed = false;

    //References
    private AudioSource AS;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (isDestroyed)
        {
            return;
        }

        Debug.Log(this.name + ": " + "Scoring Points by " + Score);
        gameManager.ScorePoints(Score);
        ObjectHitByPlayer.Invoke();
        isDestroyed = true;

        if (canSquish)
        {
            transform.DOScaleY(squishSize, squishDuration).SetEase(EaseType);

        }
    }

    public void playRandomSound()
    {
        AS.PlayOneShot(ExplosionSFX[Random.Range(0, ExplosionSFX.Length)]);
    }
}

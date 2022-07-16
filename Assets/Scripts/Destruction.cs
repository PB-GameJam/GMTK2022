using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destruction : MonoBehaviour
{
    [SerializeField] private int Score = 5;

    [SerializeField] private UnityEvent ObjectHitByPlayer;

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Debug.Log(this.name + ": " + "Scoring Points by " + Score);
        gameManager.ScorePoints(Score);
        ObjectHitByPlayer.Invoke();
    }
}

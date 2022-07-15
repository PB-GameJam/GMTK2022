using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float RoundTime;

    [Header("Menu")]
    [SerializeField] GameObject MenuView;
    [SerializeField] private CinemachineVirtualCamera MenuViewCam;

    [Header("Game")]
    [SerializeField] GameObject GameView;
    [SerializeField] private CinemachineVirtualCamera GameViewCam;

    public void Start()
    {
        MenuViewCam.Priority = 100;
        // Spawn Start Game Menu

        // Attach Start Game Method to Input
    }

    public void StartRound()
    {
        MenuViewCam.Priority = 0;
        // Remove Start Game Method to Input
    }

    public void EndRound()
    {

        // Attach Restart Method to Input
    }

    public void RestartGame()
    {
        // Remove Restart Game from Input
        // Play some animation
    }
}

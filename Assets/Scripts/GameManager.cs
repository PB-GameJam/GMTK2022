using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float RoundTime;

    [Header("UI")]
    [SerializeField] private Transform UIContainer;

    [Header("Menu")]
    [SerializeField] GameObject MenuViewPrefab;
    [SerializeField] private CinemachineVirtualCamera MenuViewCam;

    [Header("Game")]
    [SerializeField] GameObject GameViewPrefab;
    [SerializeField] private CinemachineVirtualCamera GameViewCam;
    [SerializeField] GameObject Player;

    private GameObject MenuUI;
    private GameObject GameUI;

    public void Start()
    {
        MenuViewCam.Priority = 100;
        MenuUI = Instantiate(MenuViewPrefab, UIContainer);

        Player.GetComponent<ControlPlayer>().enabled = false;
        Player.GetComponent<LaunchController>().enabled = false;
        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released += StartRound;
        
    }

    public void StartRound()
    {
        MenuViewCam.Priority = 0;

        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released -= StartRound;

        Player.GetComponent<ControlPlayer>().enabled = true;
        Player.GetComponent<LaunchController>().enabled = true;

        Destroy(MenuUI);
        GameUI = Instantiate(GameViewPrefab, UIContainer);
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

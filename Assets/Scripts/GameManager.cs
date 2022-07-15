using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float RoundTime;

    [Header("UI")]
    [SerializeField] private Transform UIContainer;

    [Header("Menu")]
    [SerializeField] private GameObject MenuViewPrefab;
    [SerializeField] private CinemachineVirtualCamera MenuViewCam;

    [Header("Game")]
    [SerializeField] private GameObject GameViewPrefab;

    [SerializeField] private CinemachineVirtualCamera GameViewCam;
    [SerializeField] private GameObject Player;

    private GameObject MenuUI;
    private GameObject GameUI;
    private float CurrentTime;
    private TextMeshProUGUI TimerTMP;

    private bool RoundRunning = false;
    public void Start()
    {
        StartGame();
    }

    public void Update()
    {
        if (RoundRunning == false)
            return;

        UpdateRoundTime();
    }

    public void StartGame()
    {
        MenuViewCam.Priority = 100;
        MenuUI = Instantiate(MenuViewPrefab, UIContainer);

        Player.GetComponent<ControlPlayer>().enabled = false;
        Player.GetComponent<LaunchController>().enabled = false;
        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released += StartRound;

        RoundRunning = false;
    }
    public void StartRound()
    {
        MenuViewCam.Priority = 0;

        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released -= StartRound;

        Player.GetComponent<ControlPlayer>().enabled = true;
        Player.GetComponent<LaunchController>().enabled = true;

        GameUI = Instantiate(GameViewPrefab, UIContainer);

        TimerTMP = GameUI.GetComponentInChildren<TimerTMP>().Timer;
        CurrentTime = RoundTime;       
        TimerTMP.text = CurrentTime.ToString();

        RoundRunning = true;

        Destroy(MenuUI);

    }

    public void UpdateRoundTime()
    {
        CurrentTime -= Time.deltaTime;
        TimerTMP.text = Mathf.Ceil(CurrentTime).ToString();

        if (CurrentTime <= 0F)
            EndRound();
    }

    

    public void EndRound()
    {
        RoundRunning = false;

        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released += RestartGame;
    }

    public void RestartGame()
    {
        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released -= RestartGame;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

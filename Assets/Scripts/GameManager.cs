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

    [Header("End")]
    [SerializeField] private GameObject EndViewPrefab;
    [SerializeField] private CinemachineVirtualCamera EndViewCam;

    private GameObject MenuUI;
    private GameObject GameUI;
    private GameObject EndUI;

    private float CurrentTime;
    private TextMeshProUGUI TimerTMP;

    private int PointsScored;
    private TextMeshProUGUI PointsTMP;

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

        //Timer TMP
        TimerTMP = GameUI.GetComponentInChildren<MenuUIValues>().Timer;
        CurrentTime = RoundTime;
        TimerTMP.text = CurrentTime.ToString();

        //Points TMP
        PointsTMP = GameUI.GetComponentInChildren<MenuUIValues>().Points;
        PointsScored = 0;
        PointsTMP.text = PointsScored.ToString();


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

    //Call this to add points
    //Player gets points by hitting things
    //Give points when the player creates desctruction
    //Has player hit this? Then give points
    //Use a event to trigger on collision. Then create different scripts that look for this descruction event. Also gives points
    public void ScorePoints(int _val)
    {
        PointsScored += _val;
        PointsTMP.text = PointsScored.ToString();
    }

    public void PauseTimer()
    {
        RoundRunning = false;
    }

    public void ResumeTimer()
    {
        RoundRunning = true;
    }

    public void EndRound()
    {
        RoundRunning = false;

        Destroy(GameUI);
        EndViewCam.Priority = 300;
        EndUI = Instantiate(EndViewPrefab, UIContainer); //This prefab is the end view

        StartCoroutine(RestartGameCR());
    }

    IEnumerator RestartGameCR()
    {
        yield return new WaitForSeconds(2F);
        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released += RestartGame;
    }

    public void RestartGame()
    {
        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released -= RestartGame;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

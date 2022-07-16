using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float RoundTime;

    [Header("UI")]
    [SerializeField] private Transform UIContainer;

    [Header("Menu")]
    [SerializeField] private GameObject MenuViewPrefab;
    [SerializeField] private CinemachineVirtualCamera MenuViewCam;
    [SerializeField] Vector3 StartingPosition;
    [SerializeField] Vector3 EndingPosition;

    [Header("Game")]
    [SerializeField] private GameObject GameViewPrefab;
    [SerializeField] private CinemachineVirtualCamera GameViewCam;
    [SerializeField] private GameObject Player;
    [SerializeField] private AudioClip StartClip;

    [Header("End")]
    [SerializeField] private GameObject EndViewPrefab;
    [SerializeField] private CinemachineVirtualCamera EndViewCam;
    [SerializeField] private AudioClip EndClip;

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

        Player.GetComponent<ControlPlayer>().enabled = false;
        Player.GetComponent<LaunchController>().enabled = false;
        Player.GetComponent<Rigidbody>().isKinematic = true;

        Player.transform.localPosition = StartingPosition;
        Player.transform.DOLocalMove(EndingPosition, 2F)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                MenuUI = Instantiate(MenuViewPrefab, UIContainer);
                MenuUI.GetComponent<CanvasGroup>().DOFade(1F, 0.5F).SetEase(Ease.OutQuad);
                Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released += StartRound;
                Player.GetComponent<Rigidbody>().isKinematic = false;
            });

        RoundRunning = false;
    }
    public void StartRound()
    {
        MenuViewCam.Priority = 0;

        AudioSource.PlayClipAtPoint(StartClip, Camera.main.transform.position);

        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released -= StartRound;
        Player.GetComponent<ControlPlayer>().enabled = true;
        Player.GetComponent<LaunchController>().enabled = true;
        Player.GetComponent<Rigidbody>().velocity = (Player.transform.forward + Vector3.up) * 20F;

        GameUI = Instantiate(GameViewPrefab, UIContainer);
        GameUI.GetComponent<CanvasGroup>().DOFade(1F, 0.5F)
            .SetEase(Ease.OutQuad);

        TimerTMP = GameUI.GetComponentInChildren<MenuUIValues>().Timer;
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

        AudioSource.PlayClipAtPoint(EndClip, Camera.main.transform.position);

        Destroy(GameUI);
        EndViewCam.Priority = 300;
        EndUI = Instantiate(EndViewPrefab, UIContainer);
        EndUI.GetComponent<CanvasGroup>().DOFade(1F, 0.5F)
            .SetEase(Ease.OutQuad);

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

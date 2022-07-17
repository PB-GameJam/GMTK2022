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
    [SerializeField] private GameObject PointsTextPrefab;
    [SerializeField] private float MinPointsPosX;
    [SerializeField] private float MaxPointsPosX;
    [SerializeField] private float MinPointsPosY;
    [SerializeField] private float MaxPointsPosY;

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

    [Header("Tutorial")]
    [SerializeField] private CinemachineVirtualCamera DiceViewCam;
    [SerializeField] private CinemachineVirtualCamera BuildingsViewCam;
    [SerializeField] private TextMeshProUGUI TutorialText;
    [SerializeField] private AudioSource TutorialSource;


    private GameObject MenuUI;
    private GameObject GameUI;
    private GameObject EndUI;

    private float CurrentTime;
    private TextMeshProUGUI TimerTMP;

    private int PointsScored;
    private TextMeshProUGUI PointsTMP;

    private TextMeshProUGUI EndPointsTMP;

    [HideInInspector] public MenuUIValues GameViewUIValues;

    private bool RoundRunning = false;

    private int TutorialIndex = 0;


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
                Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released += ViewNextTutorial;
                Player.GetComponent<Rigidbody>().isKinematic = false;
            });

        TutorialIndex = 0;
        RoundRunning = false;
    }

    public void ViewNextTutorial()
    {
        TutorialIndex++;

        switch(TutorialIndex)
        {
            case 1:
                Destroy(MenuUI);
                BuildingsViewCam.Priority = 100;
                MenuViewCam.Priority = 0;
                TutorialText.text = "Get Points By Destroying The City!";
                break;
            case 2:
                DiceViewCam.Priority = 100;
                BuildingsViewCam.Priority = 0;
                TutorialText.text = "Knock The Dice Around For Powerups And Additional Carnage!";
                break;
            case 3:
                MenuViewCam.Priority = 100;
                DiceViewCam.Priority = 0;
                TutorialText.text = "";
                Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released -= ViewNextTutorial;
                StartCoroutine(StartCountdownCR());
                break;
        }

        TutorialSource.Play();
    }

    IEnumerator StartCountdownCR()
    {
        yield return new WaitForSeconds(3F);

        StartRound();
    }

    public void StartRound()
    {
        GameViewCam.Priority = 100;
        MenuViewCam.Priority = 0;
        

        AudioSource.PlayClipAtPoint(StartClip, Camera.main.transform.position);

        Player.GetComponentInChildren<PlayerInputHandler>().Jump.Released -= StartRound;
        Player.GetComponent<ControlPlayer>().enabled = true;
        Player.GetComponent<LaunchController>().enabled = true;
        Player.GetComponent<Rigidbody>().velocity = (Player.transform.forward + Vector3.up) * 20F;

        GameUI = Instantiate(GameViewPrefab, UIContainer);
        GameUI.GetComponent<CanvasGroup>().DOFade(1F, 0.5F)
            .SetEase(Ease.OutQuad);

        //Timer TMP
        TimerTMP = GameUI.GetComponentInChildren<MenuUIValues>().Timer;
        CurrentTime = RoundTime;
        TimerTMP.text = CurrentTime.ToString();

        //Points TMP
        PointsTMP = GameUI.GetComponentInChildren<MenuUIValues>().Points;
        PointsScored = 0;
        PointsTMP.text = PointsScored.ToString();

        //store gameview UI Values script to update powerup UI
        GameViewUIValues = GameUI.GetComponent<MenuUIValues>();

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

        RectTransform pointsText = Instantiate(PointsTextPrefab, UIContainer).GetComponent<RectTransform>();
        pointsText.GetComponent<TextMeshProUGUI>().text = $"+{_val}";
        float xPos = UnityEngine.Random.Range(MinPointsPosX, MaxPointsPosX);
        float yPos = UnityEngine.Random.Range(MinPointsPosY, MaxPointsPosY);
        pointsText.anchoredPosition = new Vector2(xPos, yPos);

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

        //End Score TMP
        EndPointsTMP = EndUI.GetComponentInChildren<MenuUIValues>().Points;
        EndPointsTMP.text = PointsScored.ToString();

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

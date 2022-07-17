using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnailPowerups : MonoBehaviour
{
    [SerializeField] private float[] LaunchMultipliers = { 2f, 2.5f, 3f };
    [SerializeField] private float[] ScaleMultipliers = { 1.5f, 2f, 2.5f };
    [SerializeField] private float[] MagnetismForces = { 10f, 20f, 30f };

    [SerializeField] private float PowerupTimerMax = 30f;

    [SerializeField] private SphereCollider MagnetTrigger;

    [SerializeField] private Sprite EnlargeSprite;
    [SerializeField] private Sprite SpeedSprite;
    [SerializeField] private Sprite MagnetSprite;

    [SerializeField] private string NoPowerupText = "Roll dice to power up!";

    private List<Rigidbody> Humans;

    private LaunchController SnailLaunch;

    private GameManager GManager;

    private Vector3 OriginalScale;

    private float PowerupTimer;

    private bool HasPowerup = false;
    private DieFaces CurrentPowerup;
    private int PowerupLevel;

    public bool GetPowerup(DieFaces type)
    {
        //if this is the same as the current (active) power level / powerup, do nothing.
        if (type == DieFaces.TwoX && PowerupLevel == 2)
            return false;

        if (type == DieFaces.ThreeX && PowerupLevel == 3)
            return false;

        if (type == CurrentPowerup && HasPowerup)
            return false;

        //if this is a new level, use that with the current powerup and leave the timer.
        //otherwise, use the new type at level 1 and reset the timer.
        switch (type)
        {
            case DieFaces.TwoX:
                TurnOffPowerup();
                UpdateNewPowerup(2, CurrentPowerup, false);
                break;
            case DieFaces.ThreeX:
                TurnOffPowerup();
                UpdateNewPowerup(3, CurrentPowerup, false);
                break;
            default:
                TurnOffPowerup();
                UpdateNewPowerup(1, type, true);
                break;
        }

        return true;

        UpdateNonTimerUI();
    }

    private void UpdateNewPowerup(int level, DieFaces powerup, bool resetTimer)
    {
        HasPowerup = true;
        CurrentPowerup = powerup;
        PowerupLevel = level;

        switch (CurrentPowerup)
        {
            case DieFaces.Enlarge:
                transform.localScale = OriginalScale * ScaleMultipliers[PowerupLevel - 1];
                break;
            case DieFaces.Speed:
                SnailLaunch.LaunchSpeedMultipler = LaunchMultipliers[PowerupLevel - 1];
                break;
            default:
                break;
        }

        if (resetTimer)
            PowerupTimer = PowerupTimerMax;
    }

    private Sprite GetPowerupSprite(DieFaces type)
    {
        switch (type)
        {
            case DieFaces.Enlarge:
                return EnlargeSprite;
            case DieFaces.Speed:
                return SpeedSprite;
            case DieFaces.Magnet:
                return MagnetSprite;
            default:
                return EnlargeSprite;
        }
    }

    private string GetPowerupUIName(DieFaces type)
    {
        switch (type)
        {
            case DieFaces.Enlarge:
                return "Enlarge";
            case DieFaces.Speed:
                return "Turbo";
            case DieFaces.Magnet:
                return "Magnetize";
            default:
                return "powerup not found :/";
        }
    }

    private void TurnOffPowerup()
    {
        switch (CurrentPowerup)
        {
            case DieFaces.Enlarge:
                transform.localScale = OriginalScale;
                break;
            case DieFaces.Speed:
                SnailLaunch.LaunchSpeedMultipler = 1f;
                break;
            default:
                break;
        }

        HasPowerup = false;
        PowerupLevel = 1;
    }

    private void UpdateNonTimerUI()
    {
        if (GManager.GameViewUIValues == null)
            return;

        GManager.GameViewUIValues.PowerupLevel.gameObject.SetActive(HasPowerup);
        GManager.GameViewUIValues.PowerupTimer.gameObject.SetActive(HasPowerup);
        GManager.GameViewUIValues.PowerupIcon.gameObject.SetActive(HasPowerup);

        if (HasPowerup)
        {
            GManager.GameViewUIValues.PowerupText.text = "Powerup: " + GetPowerupUIName(CurrentPowerup);
            GManager.GameViewUIValues.PowerupLevel.text = "x" + PowerupLevel;
            GManager.GameViewUIValues.PowerupIcon.sprite = GetPowerupSprite(CurrentPowerup);
        }
        else
            GManager.GameViewUIValues.PowerupText.text = NoPowerupText;
    }

    private void Magnetize()
    {
        foreach (Rigidbody rb in Humans)
        {
            Vector3 magnetForce = transform.position - rb.position;
            magnetForce.Normalize();
            magnetForce *= MagnetismForces[PowerupLevel - 1];
            rb.AddForce(magnetForce, ForceMode.Acceleration);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SnailLaunch = GetComponent<LaunchController>();
        OriginalScale = transform.localScale;
        Humans = new List<Rigidbody>();
        GManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HasPowerup)
        {
            PowerupTimer -= Time.deltaTime;

            //update UI
            if(GManager.GameViewUIValues != null)
                GManager.GameViewUIValues.PowerupTimer.text = Mathf.Ceil(PowerupTimer).ToString() + "s";

            if (CurrentPowerup == DieFaces.Magnet)
                Magnetize();
        }

        if (PowerupTimer < 0f)
        {
            TurnOffPowerup();
            UpdateNonTimerUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HumanAI>())
            Humans.Add(other.GetComponent<Rigidbody>());
    }

    private void OnTriggerExit(Collider other)
    {
        Humans.Remove(other.GetComponent<Rigidbody>());
    }
}
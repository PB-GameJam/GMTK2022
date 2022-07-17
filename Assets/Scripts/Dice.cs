using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private float SelfKnockback;
    [SerializeField] private float MinTorque;
    [SerializeField] private float MaxTorque;
    [SerializeField] private float GravitySpeed;
    [SerializeField] private float UpwardForce;

    [Tooltip("Minimum rotation speed for the die to still be considered rolling")]
    [SerializeField] private float MinAngularVelocity = 3f;
    [SerializeField] private float MinVelocity = 1f;
    //[SerializeField] private float MovementEpsilon = 1F;
    //[SerializeField] private float DiceSideCheckTime = 4F;

    [SerializeField] private float MinTimeBetweenRolls = 1f;

    [Range(0F, 1F)] [SerializeField] private float DampeningFactor;

    [Header("Powerups on each die face, relative to its own transform")]
    [SerializeField] private DieFaces ForwardFace;
    [SerializeField] private DieFaces BackFace;
    [SerializeField] private DieFaces UpFace;
    [SerializeField] private DieFaces DownFace;
    [SerializeField] private DieFaces LeftFace;
    [SerializeField] private DieFaces RightFace;

    [Header("Powerup icons")]
    [SerializeField] private Sprite TwoSprite;
    [SerializeField] private Sprite ThreeSprite;
    [SerializeField] private Sprite EnlargeSprite;
    [SerializeField] private Sprite SpeedSprite;
    [SerializeField] private Sprite MagnetSprite;

    public event Action DiceSettled;

    private CollisionTracker CollisionTracker => Registry.Lookup<CollisionTracker>();
    private Rigidbody RBD => Registry.Lookup<Rigidbody>(this);

    private bool _HasBeenHit;
    private bool CanBeRolled = true;

    private GameObject PlayerObj;

    [SerializeField] private Canvas TopIndicatorCanvas;
    [SerializeField] private Image TopIndicatorImage;

    private void Start()
    {
        TopIndicatorCanvas.enabled = false;
    }

    private void FixedUpdate()
    {
        RBD.velocity += Vector3.down * GravitySpeed;    
    }

    private void Update()
    {
        if(_HasBeenHit)
        {
            Debug.Log("angular velocity: " + RBD.angularVelocity.magnitude + ", velocity: " + RBD.velocity.magnitude);


            //if the die's current angular velocity is below the min, check which side it's on and set to not be hit any longer.
            if (RBD.angularVelocity.magnitude < MinAngularVelocity && RBD.velocity.magnitude < MinVelocity)
            {
                DieFaces gotPowerup = CheckDiceSide(PlayerObj);

                Debug.Log(gotPowerup.ToString());
                _HasBeenHit = false;
                TopIndicatorCanvas.enabled = false;

                SnailPowerups powerupScript = PlayerObj.GetComponent<SnailPowerups>();
                if(powerupScript != null)
                {
                    powerupScript.GetPowerup(gotPowerup);
                }
            }
            else
            {
                //set canvas to look at the camera, image to display current icon
                Vector3 dirToCam = Camera.main.transform.position - transform.position;
                TopIndicatorCanvas.transform.rotation = Quaternion.LookRotation(dirToCam, Vector3.up);

                switch (CheckDiceSide(PlayerObj))
                {
                    case DieFaces.TwoX:
                        TopIndicatorImage.sprite = TwoSprite;
                        break;
                    case DieFaces.ThreeX:
                        TopIndicatorImage.sprite = ThreeSprite;
                        break;
                    case DieFaces.Enlarge:
                        TopIndicatorImage.sprite = EnlargeSprite;
                        break;
                    case DieFaces.Speed:
                        TopIndicatorImage.sprite = SpeedSprite;
                        break;
                    case DieFaces.Magnet:
                        TopIndicatorImage.sprite = MagnetSprite;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player" || !CanBeRolled)
            return;

        collision.collider.GetComponent<Rigidbody>().velocity *= DampeningFactor;
        Vector3 knockbackForce = (transform.position - collision.collider.transform.position).normalized * SelfKnockback;
        knockbackForce += Vector3.up * UpwardForce;
        Vector3 torque = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(MinTorque, MaxTorque);
        RBD.AddForce(knockbackForce, ForceMode.Impulse);
        RBD.AddTorque(torque, ForceMode.Impulse);

        GetComponentInChildren<ParticleSystem>().Play();

        _HasBeenHit = true;
        PlayerObj = collision.gameObject;
        TopIndicatorCanvas.enabled = true;

        CanBeRolled = false;
        StartCoroutine(RerollDelay());

        //StartCoroutine(CheckDiceSideCR(collision.collider.gameObject));
    }

    IEnumerator RerollDelay()
    {
        yield return new WaitForSeconds(MinTimeBetweenRolls);
        CanBeRolled = true;
    }

    //IEnumerator CheckDiceSideCR(GameObject _player)
    //{
    //    // Add some UX visuals for the dice
    //    
    //
    //    yield return new WaitForSeconds(DiceSideCheckTime);
    //
    //    CheckDiceSide(_player);
    //}

    public DieFaces CheckDiceSide(GameObject playerObj)
    {
        List<Vector3> dieFaceDirs = new List<Vector3> { transform.forward, transform.up, transform.right, -transform.forward, -transform.up, -transform.right };

        int i = 0;

        foreach (Vector3 faceDir in dieFaceDirs)
        {
            if (Vector3.Angle(Vector3.up, faceDir) < 50)
            {
                switch (i)
                {
                    case 0:
                        return ForwardFace;
                    case 1:
                        return UpFace;
                    case 2:
                        return RightFace;
                    case 3:
                        return BackFace;
                    case 4:
                        return DownFace;
                    case 5:
                        return LeftFace;
                    default:
                        return ForwardFace;
                }
            }

            i++;
        }

        //for (int i = 0; i < dieFaceDirs.Count; i++)
        //{
        //    //if the angle between the world up vector and a transform direction is < 50, that face is pointing up. Use 50 as it's slightly
        //    //larger than 45, the maximum angle
        //    if(Vector3.Angle(Vector3.up, dieFaceDirs[i]) < 50)
        //    {
        //        switch (i)
        //        {
        //            case 0:
        //                return ForwardFace;
        //            case 1:
        //                return UpFace;
        //            case 2:
        //                return RightFace;
        //            case 3:
        //                return BackFace;
        //            case 4:
        //                return DownFace;
        //            case 5:
        //                return LeftFace;
        //            default:
        //                return ForwardFace;
        //        }
        //    }
        //}

        Debug.LogError("correct upright face wasn't found, used forward face by default");
        return ForwardFace;
    }


}

public enum DieFaces
{
    TwoX, ThreeX, Enlarge, Speed, Magnet
}
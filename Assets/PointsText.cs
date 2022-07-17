using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PointsText : MonoBehaviour
{
    [SerializeField] private float ScaleUpDuration = 0.5F;
    [SerializeField] private float ScaleDownDuration = 0.5F;

    private void Start()
    {
        transform.DOScale(1F, ScaleUpDuration)
            .SetEase(Ease.InOutBack)
            .OnComplete(() => {
                
                transform.DOScale(0F, ScaleDownDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => { 
                    
                    Destroy(gameObject); });
           
            });
    }
}

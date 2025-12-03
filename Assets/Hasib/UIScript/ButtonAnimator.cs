using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation Settings")]
    public float hoverScale = 1.1f;
    public float duration = 0.15f;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
         transform.localScale= originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(hoverScale, duration).SetEase(Ease.OutBack).SetUpdate(true)
            ;
        //notificacion sfx
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlaySFX(1);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration).SetEase(Ease.OutBack).SetUpdate(true)
            ;
    }
}
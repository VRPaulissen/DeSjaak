using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DeSjaak
{
    public class BackgroundController : MonoBehaviour
    {
        private Image backgroundImage;
        private Color baseColor;

        private void Start()
        {
            EventManager.Instance.OnColorFlashRequested += FlashColor;
            backgroundImage = GetComponent<Image>();
            baseColor = backgroundImage.color;
        }

        private void OnDestroy()
        {
            EventManager.Instance.OnColorFlashRequested -= FlashColor;
        }
        
        private void FlashColor(Color newColor, float duration)
        {
            newColor.a = Mathf.Min(newColor.a, 0.5f);
            backgroundImage.DOKill();
            DOTween.Sequence()
                .Append(backgroundImage.DOColor(newColor, duration).SetEase(Ease.InOutQuad))
                .Append(backgroundImage.DOColor(baseColor, duration).SetEase(Ease.InOutQuad));
        }
    }
}

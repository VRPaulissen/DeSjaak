using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace DeSjaak
{
    public class PlayerIndicator : MonoBehaviour
    {
        [SerializeField] private SVGImage backgroundImage;
        [SerializeField] private SVGImage outerRingImage;
        [SerializeField] private SVGImage innerRingImage;
        [SerializeField] private Image fadeImage;
        
        private void OnEnable()
        {
            StartAllAnimations();
            GameManager.StateChanged += OnStateChanged;
        }
        
        private void OnDisable()
        {
            StopAllAnimations();
            GameManager.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameManager.GameState state)
        {
            backgroundImage.gameObject.SetActive(state == GameManager.GameState.ShowEnding);

            if (state == GameManager.GameState.PlayerSelection)
            {
                StartAllAnimations();
            }
            else if (state == GameManager.GameState.PlayGame)
            {
                StopOuterRingScaleAndFadeAnimation();
            }
        }

        public void Spawn()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
        }
        
        public void Despawn(TweenCallback callback)
        {
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuad).OnComplete(callback);
        }
        
        public void SetColor(Color color)
        {
            innerRingImage.color = color;
            outerRingImage.color = color;
            fadeImage.color = color;
        }

        private void StartAllAnimations()
        {
            AnimateFade();
            AnimateInnerRingScale();
            AnimateOuterRingScaleAndFade();
            AnimateRotation();
        }
        
        private void AnimateFade()
        {
            fadeImage.DOFade(0f, 1.5f).From(1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        
        private void AnimateInnerRingScale()
        {
            innerRingImage.rectTransform.localScale = Vector3.one;
            backgroundImage.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            innerRingImage.rectTransform.DOScale(1f, 1.5f)
                .From()
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            backgroundImage.rectTransform.DOScale(1.4f, 1.5f)
                .From()
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void AnimateOuterRingScaleAndFade()
        {
            outerRingImage.rectTransform.DOScale(2f, 1.5f)
                .From(new Vector3(1.1f, 1.1f, 1.1f))
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.InOutSine);

            outerRingImage.DOFade(0f, 1.5f)
                .From(1f)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.InOutSine);
        }
        
        private void AnimateRotation()
        {
            transform.DORotate(new Vector3(0f, 0f, 360f), 8f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear); 
        }

        private void StopAllAnimations()
        {
            StopFadeAnimation();
            StopInnerRingScaleAnimation();
            StopOuterRingScaleAndFadeAnimation();
            StopRotationAnimation();
        }

        private void StopFadeAnimation()
        {
            fadeImage.DOKill();
        }
        
        private void StopInnerRingScaleAnimation()
        {
            innerRingImage.DOKill();
            backgroundImage.DOKill();
        }

        private void StopOuterRingScaleAndFadeAnimation()
        {
            outerRingImage.DOKill();
            outerRingImage.color = new Color(0, 0, 0, 0);
        }

        private void StopRotationAnimation()
        {
            transform.DOKill();
        }
    }
}

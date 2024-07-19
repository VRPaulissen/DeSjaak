using System;
using DG.Tweening;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace DeSjaak
{
    public class PlayerSelectionController : MonoBehaviour
    {        
        [Header("Dependencies")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private GameManager gameManager;
        
        [Header("UI Elements")]
        [SerializeField] private SVGImage fillBackgroundImage;
        [SerializeField] private Image fillImage;

        private bool isLocked = false;
        
        private void Awake()
        {
            PlayerManager.PlayerAdded += HandlePlayerCountChanged;
            PlayerManager.PlayerRemoved += HandlePlayerCountChanged;
            GameManager.StateChanged += HandleGameStateChanged;
        }

        private void OnDestroy()
        {
            PlayerManager.PlayerAdded -= HandlePlayerCountChanged;
            PlayerManager.PlayerRemoved -= HandlePlayerCountChanged;
            GameManager.StateChanged -= HandleGameStateChanged;
        }

        private void HandlePlayerCountChanged(Player _)
        {
            if (playerManager.GetPlayerCount() > 1)
            {
                StartFillAnimation();
            }
            else
            {
                ResetAnimations();
            }
        }
        
        private void HandleGameStateChanged(GameManager.GameState state)
        {
            isLocked = state switch
            {
                GameManager.GameState.PlayerSelection => false,
                _ => true
            };
        }

        private void StartFillAnimation()
        {
            if (isLocked) return;
            
            fillImage.fillAmount = 0;
            fillBackgroundImage.DOFade(0.2f, 0f);
            fillImage.DOKill();
            fillBackgroundImage.DOKill();
            fillBackgroundImage.DOFade(1f, 3f).SetEase(Ease.Linear);
            fillImage.DOFillAmount(1f, 3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameManager.TransitionToState(GameManager.GameState.GameSelection);
            });
        }

        private void ResetAnimations()
        {
            fillImage.DOKill();
            fillBackgroundImage.DOKill();
            
            fillBackgroundImage.DOFade(0.2f, 0.25f);
            fillImage.fillAmount = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameManager.TransitionToState(GameManager.GameState.GameSelection);
            }
        }
    }
}

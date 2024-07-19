using System;
using System.Collections;
using System.Collections.Generic;
using DeSjaak.Managers;
using UnityEngine;

namespace DeSjaak
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private GameObject playerSelectionScreen;
        [SerializeField] private GameObject gameSelectionScreen;
        [SerializeField] private GameObject endScreen;

        private void Awake()
        {
            GameManager.StateChanged += HandleGameStateChange;
        }

        private void Start()
        {
            HandleGameStateChange(GameManager.GameState.PlayerSelection);
        }

        private void OnDestroy()
        {
            GameManager.StateChanged -= HandleGameStateChange;
        }
        
        private void HandleGameStateChange(GameManager.GameState state)
        {
            DeactivateAllScreens();

            switch (state)
            {
                case GameManager.GameState.PlayerSelection:
                    playerSelectionScreen.SetActive(true);
                    break;
                case GameManager.GameState.GameSelection:
                    gameSelectionScreen.SetActive(true);
                    break;
                case GameManager.GameState.PlayGame:
                    break;
                case GameManager.GameState.ShowEnding:
                    endScreen.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void DeactivateAllScreens()
        {
            playerSelectionScreen.SetActive(false);
            gameSelectionScreen.SetActive(false);
            endScreen.SetActive(false);
        }
    }
}

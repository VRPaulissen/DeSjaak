using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeSjaak
{
    public class GameManager : MonoBehaviour
    {
        #region Enums
        
        public enum GameEndCondition
        {
            SuddenDeath,
            LastManStanding,
        }
        
        public enum GameState
        {
            PlayerSelection,
            GameSelection,
            PlayGame,
            ShowEnding,
        }
        #endregion

        #region Fields
        [Header("Game Configurations")]
        [SerializeField] private List<GameConfig> gameConfigs;
        [SerializeField] private Transform canvasParent;

        private Game activeGame;
        private GameConfig selectedGameConfig;

        public static event Action<GameState> StateChanged;
        public static event Action<Color> GameEnded;
        
        public static GameState CurrentState { get; set; }
        public static GameEndCondition EndCondition { get; private set; }

        public static void SetEndCondition(GameEndCondition endCondition)
        {
            EndCondition = endCondition;
        }
        
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            TransitionToState(GameState.PlayerSelection);
        }
        #endregion
        
        #region State Management
        public void TransitionToState(GameState newState)
        {
            CurrentState = newState;
            StateChanged?.Invoke(CurrentState);

            switch (newState)
            {
                case GameState.PlayGame:
                    StartCoroutine(PlayGameRoutine());
                    break;
                case GameState.ShowEnding:
                    StartCoroutine(ShowLoserRoutine());
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Handlers for Each GameState

        private IEnumerator PlayGameRoutine()
        {
            activeGame = Instantiate(selectedGameConfig.gamePrefab, canvasParent).GetComponent<Game>();
            activeGame.InitializeGame();
            activeGame.StartGame();
            
            yield return new WaitUntil(() => activeGame.IsGameCompleted);
            
            GameEnded?.Invoke(activeGame.LoserColor);
            TransitionToState(GameState.ShowEnding);
        }

        private IEnumerator ShowLoserRoutine()
        {
            activeGame.gameObject.SetActive(false);
            
            yield return new WaitForSeconds(4f);
            
            if (activeGame != null)
            {
                Destroy(activeGame.gameObject);
                activeGame = null;
            }
            
            TransitionToState(GameState.PlayerSelection);

        }
        #endregion

        #region Configuration Management
        public int GetConfigCount() => gameConfigs.Count;
        
        public string GetConfigName(int index)
        {
            return gameConfigs[index].gameName;
        }
        
        public void SetConfig(int gameIndex)
        {
            if (gameIndex < 0 || gameIndex >= gameConfigs.Count) return;
            selectedGameConfig = gameConfigs[gameIndex];
        }
        #endregion
    }
}

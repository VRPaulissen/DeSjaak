using System.Collections;
using System.Collections.Generic;
using DeSjaak.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DeSjaak
{
    public class GameSelectionController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameManager gameManager;
        
        [Header("UI Elements")]
        [SerializeField] private TMP_Text selectedGameTitle;


        [Header("Debug")] 
        [SerializeField] private bool selectSpecificGame = false;
        [SerializeField] private int selectGame = 0;
        
        private int targetGame = 0;

        private const float MIN_DELAY = 0.01f;
        private const float MAX_DELAY = 0.3f;
        private const float FADE_DURATION = 0.1f;
        private const float POST_SELECTION_PAUSE = 0.55f;
        private const float FINAL_OPACITY = 1f;
        private const float POST_FADE_PAUSE = 2f;

        public void OnEnable()
        {
            StartCoroutine(StartSelection());
        }

        private IEnumerator StartSelection()
        {
            var totalSpins = Random.Range(gameManager.GetConfigCount() * 3, gameManager.GetConfigCount() * 5);
            var currentDelay = MIN_DELAY;

            for (var i = 0; i < totalSpins; i++)
            {
                targetGame = i % gameManager.GetConfigCount();
                selectedGameTitle.text = gameManager.GetConfigName(targetGame);
                FadeTextToOpacity(0.2f + 0.8f * (i / (float)(totalSpins - 1)), FADE_DURATION);

                yield return new WaitForSeconds(currentDelay);
                currentDelay = Mathf.Lerp(currentDelay, MAX_DELAY, 0.05f);
            }
            
            yield return new WaitForSeconds(POST_SELECTION_PAUSE);
            UpdateTargetGame();

            yield return new WaitForSeconds(POST_FADE_PAUSE);
            gameManager.TransitionToState(GameManager.GameState.PlayGame);
        }

        private void FadeTextToOpacity(float targetOpacity, float duration)
        {
            selectedGameTitle.DOFade(targetOpacity, duration).SetEase(Ease.Linear);
        }

        private void UpdateTargetGame()
        {
            targetGame = selectSpecificGame ? selectGame : (targetGame + 1) % gameManager.GetConfigCount();
            gameManager.SetConfig(targetGame);
            selectedGameTitle.text = gameManager.GetConfigName(targetGame);
            selectedGameTitle.DOFade(FINAL_OPACITY, FADE_DURATION);
        }
    }
}

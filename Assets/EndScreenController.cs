using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeSjaak
{
    public class EndScreenController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text backgroundText;

        private void Awake()
        {
            GameManager.GameEnded += HandleGameEnded;
        }

        private void OnDestroy()
        {
            GameManager.GameEnded -= HandleGameEnded;
        }

        private void HandleGameEnded(Color color)
        {
            backgroundImage.color = new Color32(22, 22, 22, 255);
                
            if (GameManager.EndCondition == GameManager.GameEndCondition.SuddenDeath)
            {
                backgroundText.text = "LOSER";
                playerManager.RemoveAllPlayersExceptOne(color);
            }
            else
            {
                backgroundText.text = "WINNER";
            }
            
            DOTween.Sequence()
                .Append(backgroundImage.DOColor(color, 0.5f))
                .AppendInterval(3f)
                .Append(backgroundImage.DOColor(new Color32(22, 22, 22, 255), 0.5f));
        }
    }
}

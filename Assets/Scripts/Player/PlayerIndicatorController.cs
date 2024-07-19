using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace DeSjaak
{
    public class PlayerIndicatorController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerManager playerManager;
        
        [Header("UI Elements")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject playerIndicatorPrefab;
        [SerializeField] private TMP_Text playerCountText;
        
        public Dictionary<int, GameObject> PlayerIndicators { get; } = new(); 
        private IObjectPool<GameObject> indicatorPool;
        
        private void Awake()
        {
            indicatorPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(playerIndicatorPrefab, canvas.transform),
                actionOnGet: indicator => indicator.SetActive(true),
                actionOnRelease: indicator => indicator.SetActive(false),
                actionOnDestroy: Destroy,
                defaultCapacity: 10,
                maxSize: 10
            );
        }

        private void OnEnable()
        {
            PlayerManager.PlayerAdded += AddPlayerIndicator;
            PlayerManager.PlayerRemoved += RemovePlayerIndicator;
        }

        private void OnDisable()
        {
            PlayerManager.PlayerAdded -= AddPlayerIndicator;
            PlayerManager.PlayerRemoved -= RemovePlayerIndicator;
        }

        private void AddPlayerIndicator(Player player)
        {
            if (!PlayerIndicators.ContainsKey(player.Id))
            {
                var indicatorObject = indicatorPool.Get();
                PlayerIndicators[player.Id] = indicatorObject;
                
                var playerIndicator = indicatorObject.GetComponent<PlayerIndicator>();
                playerIndicator.SetColor(player.Color);
                playerIndicator.Spawn();
            }

            UpdatePlayerCount();
        }
        
        private void RemovePlayerIndicator(Player player)
        {
            if (PlayerIndicators.Remove(player.Id, out var indicatorObject))
            {
                var playerIndicator = indicatorObject.GetComponent<PlayerIndicator>();
                playerIndicator.Despawn(() =>
                {
                    indicatorPool.Release(indicatorObject);
                });
            }

            UpdatePlayerCount();
        }

        public void UpdateIndicatorPosition(Touch touch)
        {
            if (playerManager.PlayersById.TryGetValue(touch.fingerId, out var player))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, touch.position, canvas.worldCamera, out var localPoint);
                player.UpdatePosition(localPoint);
                PlayerIndicators[touch.fingerId].transform.localPosition = localPoint;
            }
        }
        
        private void UpdatePlayerCount()
        {
            var playerCount = playerManager.GetPlayerCount();
            playerCountText.text = $"{playerCount} PLAYERS";
        }
    }
}
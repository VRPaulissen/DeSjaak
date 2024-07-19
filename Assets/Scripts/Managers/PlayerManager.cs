using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DeSjaak
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerIndicatorController playerIndicatorController;
        [SerializeField] private Canvas canvas;
        
        [Header("Other")]
        [SerializeField] private List<Color> playerColors;

        public static event Action<Player> PlayerAdded;
        public static event Action<Player> PlayerRemoved;

        public Dictionary<int, Player> PlayersById { get; set; }= new();
        private Queue<Color> availableColors = new();
        private bool isLocked = false;
        
        #region Unity Lifecycle

        private void Awake()
        {
            foreach (var color in playerColors)
            {
                availableColors.Enqueue(color);
            }
            
            GameManager.StateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.StateChanged -= HandleStateChanged;
        }

        private void Update()
        {
            HandleTouches();
        }

        #endregion

        #region Player Handling
        
        private void HandleTouches()
        {
            foreach (var touch in Input.touches)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) && IsTouchOnInteractiveUI(touch)) continue;
                
                if (touch.phase == TouchPhase.Began && !PlayersById.ContainsKey(touch.fingerId) && !isLocked && availableColors.Count > 0)
                {
                    AddPlayer(touch);
                }
                else if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled && PlayersById.ContainsKey(touch.fingerId) && !isLocked)
                {
                    RemovePlayer(touch.fingerId);
                }

                playerIndicatorController.UpdateIndicatorPosition(touch);
            }
        }
        
        private void HandleStateChanged(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.PlayerSelection:
                    isLocked = false;
                    RemoveAllPlayers();
                    break;
                default:
                    isLocked = true;
                    break;
            }
        }
        
        private void AddPlayer(Touch touch)
        {
            var color = availableColors.Dequeue();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, touch.position, canvas.worldCamera, out var localPoint);
            var player = new Player(touch.fingerId, color, localPoint);
            PlayersById[touch.fingerId] = player;
            PlayerAdded?.Invoke(player);
        }

        private void RemovePlayer(int fingerId)
        {
            if (PlayersById.Remove(fingerId, out var player))
            {
                availableColors.Enqueue(player.Color);
                PlayerRemoved?.Invoke(player);
            }
        }
        
        private void RemoveAllPlayers()
        {
            var keysToRemove = PlayersById.Keys.ToList();

            foreach (var id in keysToRemove)
            {
                if (PlayersById.Remove(id, out var player))
                {
                    availableColors.Enqueue(player.Color);
                    PlayerRemoved?.Invoke(player);
                }
            }
        }
        
        public void RemoveAllPlayersExceptOne(Color color)
        {
            var playersToRemove = PlayersById.Values.Where(player => player.Color != color).ToList();
            foreach (var player in playersToRemove)
            {
                RemovePlayer(player.Id);
            }
        }
        
        public void RemovePlayerByColor(Color color)
        {
            var playerToRemove = PlayersById.Values.FirstOrDefault(player => color == player.Color);
            if (playerToRemove != null)
            {
                RemovePlayer(playerToRemove.Id);
            }
        }
        
        public void RemoveOtherPlayersThatAreNotColor(List<Color> colors)
        {
            var playersToRemove = PlayersById.Values.Where(player => !colors.Contains(player.Color)).ToList();
            foreach (var player in playersToRemove)
            {
                RemovePlayer(player.Id);
            }
        }
        
        public Color GetSinglePlayerColor()
        {
            if (PlayersById.Count == 1)
            {
                return PlayersById.Values.First().Color;
            }
            
            throw new Exception($"More than one player left {PlayersById.Count}");
        }
        
        public int GetPlayerCount()
        {
            return PlayersById.Count;
        }
        
        private bool IsTouchOnInteractiveUI(Touch touch)
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(touch.position.x, touch.position.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Any(result => result.gameObject.CompareTag($"InteractiveUI"));
        }

        #endregion
    }
}

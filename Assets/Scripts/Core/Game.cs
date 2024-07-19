using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeSjaak
{
    public abstract class Game : MonoBehaviour
    {
        #region Fields and Properties
        private PlayerManager playerManager;
        public Color LoserColor { get; protected set; }
        public bool IsGameCompleted { get; protected set; } = false;
        
        #endregion
        
        #region Unity Methods
        public virtual void InitializeGame()
        {
            playerManager = FindObjectOfType<PlayerManager>();
            if (playerManager == null)
            {
                Debug.LogWarning("No instance for PlayerManager found.");
            }
            
            gameObject.SetActive(true);
            IsGameCompleted = false;
        }
        public abstract void StartGame();

        protected void RemovePlayer(Color loserColor)
        {
            playerManager.RemovePlayerByColor(loserColor);
        }
        
        protected void RemoveOtherPlayers(List<Color> loserColor)
        {
            playerManager.RemoveOtherPlayersThatAreNotColor(loserColor);
        }
        
        public abstract void EndGame(Color loserColor);
        #endregion

        #region Helper Methods
        protected Dictionary<Player, Vector3> GetPlayerPositions()
        {
            return playerManager.PlayersById.Values.ToDictionary(player => player, player => player.Position);
        }

        protected Color GetWinnerColor()
        {
            return playerManager.GetSinglePlayerColor();
        }

        protected int GetPlayerCount()
        {
            return playerManager.PlayersById.Count;
        }
        #endregion
        
    }
}

using UnityEngine;

namespace DeSjaak
{
    public class GameFactory : MonoBehaviour
    {
        [SerializeField] private Transform canvasParent;
        
        public Game CreateGame(GameConfig config)
        {
            var gamePrefab = Instantiate(config.gamePrefab, canvasParent);
            var game = gamePrefab.GetComponent<Game>();
            return game;
        }
    }
}

using UnityEngine;

namespace DeSjaak
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
    public class GameConfig : ScriptableObject
    {
        public string gameName;
        public GameObject gamePrefab;
    }
}

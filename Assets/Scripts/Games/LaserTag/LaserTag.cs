using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace DeSjaak
{
    public class LaserTag : Game
    {
        [Header("Prefabs")]
        [SerializeField] private LaserController laserPrefab;
        
        private ObjectPool<LaserController> laserPool;
        [SerializeField] private List<LaserController> activeLasers = new ();
        private Coroutine gameLoopCoroutine;
        
        private const float INITIAL_DELAY = 1f;
        private const float MINIMUM_DELAY = 0.25f; 
        private const float ACCELERATION_INTERVAL = 5f;
        private const float ACCELERATION_FACTOR = 0.9f;
        private const float PROXIMITY_THRESHOLD = 200.0f;

        private float gameElapsedTime = 0f;
        List<Player> hitPlayers = new ();
        
        #region Game Logic
        private void Awake()
        {
            laserPool = new ObjectPool<LaserController>(
                createFunc: () => Instantiate(laserPrefab, transform),
                actionOnGet: laser => laser.gameObject.SetActive(true),
                actionOnRelease: laser => laser.gameObject.SetActive(false),
                actionOnDestroy: laser => Destroy(laser.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 20
            );
        }
        
        public override void StartGame()
        {
            gameLoopCoroutine = StartCoroutine(GameLoop());
        }
                
        public override void EndGame(Color loserColor)
        {
            LoserColor = loserColor;
            if (gameLoopCoroutine != null)
            {
                StopCoroutine(gameLoopCoroutine);
            }
            
            IsGameCompleted = true;
        }

        private IEnumerator GameLoop()
        {
            var currentDelay = INITIAL_DELAY;
            var nextAccelerationTime = ACCELERATION_INTERVAL;
            
            while (!IsGameCompleted)
            {
                SpawnLaser();
                yield return new WaitForSeconds(currentDelay);
                gameElapsedTime += currentDelay;
                if (gameElapsedTime >= nextAccelerationTime)
                {
                    currentDelay *= ACCELERATION_FACTOR;
                    currentDelay = Mathf.Max(currentDelay, MINIMUM_DELAY);
                    nextAccelerationTime += ACCELERATION_INTERVAL;
                }
            }
        }

        private void SpawnLaser()
        {
            var laser = laserPool.Get();
            laser.Setup();
            laser.LaserActivated += HandleLaserActivated;
            laser.LaserDeactivated += HandleLaserDeactivated;
        }
        
        private void HandleLaserActivated(LaserData laserData)
        {
            if (laserData.Sender != null)
            {
                activeLasers.Add(laserData.Sender);
                laserData.Sender.LaserActivated -= HandleLaserActivated;
            }
        }
        
        private void HandleLaserDeactivated(LaserController laserController)
        {
            if (laserController != null)
            {
                laserController.LaserDeactivated -= HandleLaserDeactivated;
                laserController.LaserActivated -= HandleLaserActivated;
                laserPool.Release(laserController);
                activeLasers.Remove(laserController);
            }
        }

        private void FixedUpdate()
        {
            if (activeLasers.Count == 0) return; 
            hitPlayers = new List<Player>();

            foreach (var laser in activeLasers)
            {
                CheckLaserInteractions(laser);
            }

            if (hitPlayers.Count <= 0) return;
            
            if (hitPlayers.Count > 1)
            {
                if (GameManager.EndCondition == GameManager.GameEndCondition.SuddenDeath)
                {
                    var loserColors = hitPlayers.Select(player => player.Color).ToList();
                    RemoveOtherPlayers(loserColors);
                    return;
                }
            }

            foreach (var player in hitPlayers)
            {
                HandlePlayerHit(player);
                if (IsGameCompleted)
                {
                    break;
                }
            }
        }
        
        private void CheckLaserInteractions(LaserController laser)
        {
            var (startPosition, endPosition) = laser.GetPositions();
            foreach (var (player, position) in GetPlayerPositions())
            {
                if (CheckProximityToLaser(position, startPosition, endPosition))
                {
                    hitPlayers.Add(player);
                }
            }
        }

        private bool CheckProximityToLaser(Vector2 playerPosition, Vector2 laserStart, Vector2 laserEnd)
        {
            var numerator = Mathf.Abs((laserEnd.y - laserStart.y) * playerPosition.x - (laserEnd.x - laserStart.x) * playerPosition.y + laserEnd.x * laserStart.y - laserEnd.y * laserStart.x);
            var denominator = Mathf.Sqrt((laserEnd.y - laserStart.y) * (laserEnd.y - laserStart.y) + (laserEnd.x - laserStart.x) * (laserEnd.x - laserStart.x));
            var distance = numerator / denominator;
            
            return distance <= PROXIMITY_THRESHOLD;
        }
        
        private void HandlePlayerHit(Player player)
        {
            if (GameManager.EndCondition == GameManager.GameEndCondition.SuddenDeath)
            {
                EndGame(player.Color);
                return;
            }

            RemovePlayer(player.Color);
            EventManager.Instance.RequestColorFlash(player.Color, 0.2f);

            if (GetPlayerCount() <= 1)
            {
                var winnerColor = GetWinnerColor();
                EndGame(winnerColor);
            }
        }
        
        
        #endregion

    }
}

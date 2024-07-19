using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DeSjaak
{
    public class LaserController : MonoBehaviour
    {
        [Header("Laser Objects")] 
        [SerializeField] private List<GameObject> laserDotObjects;
        [SerializeField] private GameObject laserLineObject;

        [Header("Animation Properties")] 
        [SerializeField] private float fadeInDuration = 0.35f;
        [SerializeField] private float fadeOutDuration = 0.25f;

        public event Action<LaserData> LaserActivated;
        public event Action<LaserController> LaserDeactivated;

        private Image lineImage;
        private RectTransform parentRectTransform, lineRectTransform;
        private Vector2 startPosition, endPosition;
        private Side startSide, endSide;
        private readonly List<Color> colorPalette = new()
        {
            Color.red, Color.blue, Color.green, Color.yellow,
            Color.cyan, Color.magenta, new Color(1f, 0.5f, 0f),
            new Color(0.5f, 0f, 1f), new Color(1f, 0.1f, 0.5f),
            new Color(0f, 0.5f, 1f), new Color(0.1f, 1f, 0.5f),
            new Color(0.5f, 1f, 0.1f), new Color(1f, 0.1f, 0.1f),
            new Color(1f, 0.5f, 0.5f), new Color(0.1f, 0.1f, 1f),
            new Color(1f, 1f, 0.5f), new Color(0.5f, 0.5f, 1f)
        };

        private void Awake()
        {
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
            lineRectTransform = laserLineObject.GetComponent<RectTransform>();
            lineImage = laserLineObject.GetComponent<Image>();
        }

        public void Setup()
        {
            SelectRandomSides();
            AssignRandomColor();
            ConfigureLaserPositions();
            StartAnimation();
        }
        
        private void ConfigureLaserPositions()
        {
            var halfWidth = parentRectTransform.rect.width / 2 - 50;
            var halfHeight = parentRectTransform.rect.height / 2 - 50;
            
            startPosition = GetRandomSidePosition(startSide, halfWidth, halfHeight);
            endPosition = GetRandomSidePosition(endSide, halfWidth, halfHeight);
            
            laserDotObjects[0].transform.localPosition = startPosition;
            laserDotObjects[1].transform.localPosition = endPosition;
            PositionLineBetweenDots();
        }
        
        private Vector2 GetRandomSidePosition(Side side, float halfWidth, float halfHeight) => side switch
        {
            Side.Top => new Vector2(Random.Range(-halfWidth, halfWidth), halfHeight),
            Side.Bottom => new Vector2(Random.Range(-halfWidth, halfWidth), -halfHeight),
            Side.Left => new Vector2(-halfWidth, Random.Range(-halfHeight, halfHeight)),
            Side.Right => new Vector2(halfWidth, Random.Range(-halfHeight, halfHeight)),
            _ => Vector2.zero
        };
        
        private void PositionLineBetweenDots()
        {
            var midpoint = (startPosition + endPosition) / 2;
            var distance = Vector2.Distance(startPosition, endPosition);
            var angle = Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * Mathf.Rad2Deg;
            
            lineRectTransform.localPosition = midpoint;
            lineRectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            lineRectTransform.sizeDelta = new Vector2(distance, 50);
        }

        private void SelectRandomSides()
        {
            startSide = (Side)Random.Range(0, 4);
            endSide = (Side)Random.Range(0, 4);
            
            if (endSide == startSide)
            {
                endSide = (Side)(((int)startSide + 1) % 4);
            }
        }

        private void StartAnimation()
        {
            lineImage.color = new Color(lineImage.color.r, lineImage.color.g, lineImage.color.b, 0.3f);
            var activeDuration = Random.Range(1f, 2f);
            var laserData = new LaserData(this, startPosition, endPosition, activeDuration);
            
            DOTween.Sequence()
                .AppendInterval(Random.Range(1f, 2f))
                .Append(lineImage.DOFade(1f, fadeInDuration))
                .AppendCallback(() => LaserActivated?.Invoke(laserData))
                .AppendInterval(activeDuration)
                .Append(lineImage.DOFade(0f, fadeOutDuration))
                .AppendCallback(() => LaserDeactivated?.Invoke(this));
        }
        private void AssignRandomColor()
        {
            if (colorPalette.Count == 0) return;
            var randomColor = colorPalette[Random.Range(0, colorPalette.Count)];
            foreach (var dotImage in laserDotObjects.Select(dot => dot.GetComponent<Image>()))
            {
                if (dotImage == null) continue;
                dotImage.color = randomColor;
            }
            
            lineImage.color = randomColor;
        }

        public (Vector2 StartPosition, Vector2 EndPosition) GetPositions()
        {
            return (startPosition, endPosition);
        }
    }

    public struct LaserData
    {
        public LaserController Sender;
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public float Duration;

        public LaserData(LaserController sender, Vector2 startPosition, Vector2 endPosition, float duration)
        {
            Sender = sender;
            StartPosition = startPosition;
            EndPosition = endPosition;
            Duration = duration;
        }
    }

    public enum Side
    {
        Top,
        Bottom,
        Left,
        Right
    }
}
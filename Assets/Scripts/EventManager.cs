using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeSjaak
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }
        
        public delegate void ColorChangeRequest(Color color, float duration);
        public event ColorChangeRequest OnColorFlashRequested;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RequestColorFlash(Color color, float duration)
        {
            OnColorFlashRequested?.Invoke(color, duration);
        }
    }
}

using System;
using UnityEngine;

namespace DeSjaak
{
    [Serializable] public class Player
    {
        public int Id { get; private set; }
        public Color Color { get; private set; }
        public Vector3 Position { get; private set; }
        
        public Player(int id, Color color, Vector3 position)
        {
            Id = id;
            Color = color;
            Position = position;
        }
        
        public void UpdatePosition(Vector3 position)
        {
            Position = position;
        }

        public void UpdateId(int id)
        {
            Id = id;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeSjaak
{
    public class FluidBackground : MonoBehaviour
    {
        public Material interactionMaterial;
        public List<Transform> sprites;
        public float radius = 1.0f;

        private void Update()
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (i < 10)
                {
                    interactionMaterial.SetVector("_SpritePositions" + i, sprites[i].position);
                }
            }
            interactionMaterial.SetFloat("_Radius", radius);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeSjaak.Managers
{
    public class PlayerController : MonoBehaviour
    {
        public Color playerColor;
        public float moveSpeed = 5f;

        private Rigidbody2D rb;
        private SpriteRenderer sr;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            sr.material = new Material(Shader.Find("Custom/FluidShader"));
            sr.material.SetColor("_Color", playerColor);
        }

        private void Update()
        {
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            rb.velocity = movement * moveSpeed;
        }
    }
}

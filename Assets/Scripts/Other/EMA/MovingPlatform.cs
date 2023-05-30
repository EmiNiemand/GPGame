using System;
using UnityEngine;

namespace Other.EMA
{
    public class MovingPlatform : MonoBehaviour
    {
        [Tooltip("Relative to start position")]
        [SerializeField] private Vector3 endPosition;

        private Vector3 startPosition;
        private Rigidbody2D rigidbody2D;

        private bool inverseDirection = false;

        private void Start()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            
            startPosition = transform.position;
            endPosition += startPosition;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = new Vector2(1, 0);
            rigidbody2D.MovePosition(rigidbody2D.position + velocity * ((inverseDirection?-1:1) * Time.fixedDeltaTime));

            if (Vector3.Distance(transform.position, startPosition) < 0.1f) inverseDirection = true;
            else if (Vector3.Distance(transform.position, endPosition) < 0.1f) inverseDirection = false;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.contacts[0].point.y < transform.position.y) return;

            collision.rigidbody.velocity += rigidbody2D.velocity;
        }
    }
}
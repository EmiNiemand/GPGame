using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enemies
{
    public class Hedgehog : Enemy
    {        
        private Vector3 spriteSize;

        private bool bIsHit = false;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            spriteSize = spriteRenderer.bounds.size;
        }

        void FixedUpdate()
        {
            if (bIsHit)
            {
                if (!Utils.ShootBoxcast(transform.position, new Vector2(0.1f,
                            spriteSize.y / 2 + 0.2f), Vector2.right * -lookingDirection,
                        Mathf.Abs(spriteSize.x / 2), "Environment") ||
                    !Utils.ShootBoxcast(transform.position, new Vector2(0.1f,
                            spriteSize.y / 2 + 0.2f), Vector2.right * lookingDirection,
                        Mathf.Abs(spriteSize.x / 2), "Environment"))
                {
                    rb2D.velocity = new Vector2(0, 0);
                }
                return;
            }
            
            rb2D.velocity = new Vector2(moveSpeed * lookingDirection, 0);
            if (Utils.ShootBoxcast(transform.position, new Vector2(0.1f,
                        spriteSize.y / 2 - 0.2f), Vector2.right * lookingDirection,
                    Mathf.Abs(spriteSize.x / 2), "Environment"))
            {
                ChangeLookingDirection();
            }
            else if (!Utils.ShootRaycast(transform.position, 
                         new Vector2(lookingDirection, -1),
                         new Vector2(spriteSize.x, spriteSize.y).magnitude / 4 + 0.2f, "Environment"))
            {
                ChangeLookingDirection();
            }
        }

        public override void ReceiveDamage(int damage, Vector2 sourcePoint = default, int knockbackForce=0)
        {
            base.ReceiveDamage(damage, sourcePoint, knockbackForce);
            // Throw back enemy after being hit
            if(knockbackForce > 0)
            {
                Vector2 positionDiff = (rb2D.position - sourcePoint);
                // Do not move enemy vertically
                positionDiff.y = 0;
                rb2D.AddForce(positionDiff.normalized * knockbackForce);
            }
            StartCoroutine(IsHit());
        }

        private IEnumerator IsHit()
        {
            bIsHit = true;
            yield return new WaitForSeconds(0.69f);
            bIsHit = false;
        }
    }
}
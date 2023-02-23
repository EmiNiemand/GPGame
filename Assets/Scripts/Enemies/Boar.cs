using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Enemies
{
    public class Boar : Enemy
    {
        private enum BoarStates
        {
            Idle,
            Charge,
            Rush,
            Stun
        }

        [Header("Boar properties")]
        [SerializeField] private float chargeTime = 0.5f;
        [SerializeField] private float stunTime = 1.5f;

        private BoarStates currentState;
        private BoarStates previousState;
        
        private Vector3 spriteSize;

        private bool bHitWall = false;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            currentState = BoarStates.Idle;
            spriteSize = spriteRenderer.bounds.size;
        }
        
        void FixedUpdate()
        {
            OnStayState();
        }

        // State machine methods
        private void SetCurrentState(BoarStates state)
        {
            previousState = currentState;
            currentState = state;
            if (currentState != previousState)
            {
                OnExitState(previousState);
                OnEnterState(currentState);
            }
        }
        
        private void OnEnterState(BoarStates state)
        {
            switch (state)
            {
                case BoarStates.Charge:
                    StartCoroutine(Charge());
                    break;
                case BoarStates.Rush:
                    StartCoroutine(Rush());
                    break;
                case BoarStates.Stun:
                    StartCoroutine(Stun());
                    break;
                default: break;
            }
        }

        private void OnStayState()
        {
            switch (currentState)
            {
                case BoarStates.Idle:
                    if (Utils.ShootBoxcast(transform.position, new Vector2(0.2f,
                                spriteSize.y / 2 + 0.5f), Vector2.right * lookingDirection,
                            Mathf.Abs(spriteSize.x / 2) - 0.8f, "Environment"))
                    {
                        ChangeLookingDirection();
                    }
                    else if (!Utils.ShootRaycast(transform.position, 
                                 new Vector2(lookingDirection, -1),
                                 new Vector2(spriteSize.x, spriteSize.y).magnitude / 4 + 0.5f, "Environment"))
                    {
                        ChangeLookingDirection();
                    }
                    rb2D.velocity = new Vector2(moveSpeed / 5 * lookingDirection, 0);
                    break;
                case BoarStates.Rush:
                    bHitWall = Utils.ShootBoxcast(transform.position, new Vector2(0.2f,
                            spriteSize.y / 2 + 0.5f), Vector2.right * lookingDirection,
                        Mathf.Abs(spriteSize.x / 2) - 0.8f, "Environment") || 
                               !Utils.ShootRaycast(transform.position, 
                                   new Vector2(lookingDirection, -1),
                        new Vector2(spriteSize.x, spriteSize.y).magnitude / 4 + 0.5f, "Environment");
                    rb2D.velocity = new Vector2(moveSpeed * lookingDirection, 0);
                    break;
                default: break;
            }
        }

        private void OnExitState(BoarStates state)
        {
            switch (state)
            {
                case BoarStates.Rush:
                    bHitWall = false;
                    rb2D.velocity = Vector2.zero;
                    break;
                default: break;
            }
        }

        // IEnumerator methods
        private IEnumerator Charge()
        {
            yield return new WaitForSeconds(chargeTime);
            SetCurrentState(BoarStates.Rush);
        }

        private IEnumerator Rush()
        {
            yield return new WaitUntil(() => bHitWall);
            SetCurrentState(BoarStates.Stun);
        }

        private IEnumerator Stun()
        {
            yield return new WaitForSeconds(stunTime);
            ChangeLookingDirection();
            SetCurrentState(BoarStates.Idle);
        }

        // Trigger
        private void OnTriggerStay2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player") && currentState == BoarStates.Idle)
            {
                SetCurrentState(BoarStates.Charge);
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
        }
    }
}


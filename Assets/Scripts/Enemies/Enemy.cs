using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Enemies
{

    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        // IDamageable variables
        [Header("Combat")]
        [field: SerializeField] public int damage;
        [field: SerializeField] public int knockbackForce;
        [field: SerializeField] public int maxHP {get; protected set;}
        public int HP {get; protected set;}


        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 3.0f;


        [Header("Flashing properties")]
        [SerializeField] private bool isFlashing = false;

        [Tooltip("Values from 0-100, indicates how much of damage color should be shown; 0 - none, 100 - all")]
        [SerializeField] private float flashingStage = 0.0f;

        [SerializeField] public float flashingSpeed = 10.0f;
        [SerializeField] private bool flashingBackwards = false;
        [SerializeField] private Color flashingColor = Color.red;
        [SerializeField] private Color basicColor;

        [Header("Special effects")]
        [SerializeField] private GameObject deathEffect;

        protected Rigidbody2D rb2D;
        protected SpriteRenderer spriteRenderer;
        protected EnemyUI enemyUI;
        public int lookingDirection {get; protected set;}

        // Start is called before the first frame update
        protected virtual void Start()
        {
            gameObject.tag = "Enemy";
            HP = maxHP;

            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyUI = GetComponentInChildren<EnemyUI>();
            enemyUI.Setup();
            enemyUI.SetMaxHP(maxHP);
            lookingDirection = GetComponentInParent<EnemySpawner>().lookingDirection;

            basicColor = spriteRenderer.color;
            gameObject.transform.localScale = new Vector3(Mathf.Abs(gameObject.transform.localScale.x) * lookingDirection, gameObject.transform.localScale.y, 1);

            knockbackForce *= Utils.KnockbackMult;
        }

        // Update is called once per frame
        void Update()
        {
            if (isFlashing)
            {
                if (flashingBackwards)
                {
                    flashingStage -= flashingSpeed;
                }
                else
                {
                    flashingStage += flashingSpeed;
                }
                
                if (flashingStage >= 100)
                {
                    flashingBackwards = true;
                    flashingStage = 100;
                } else if (flashingStage <= 0)
                {
                    isFlashing = false;
                    flashingBackwards = false;
                    flashingStage = 0;
                }

                // _spriteRenderer.color = basicColor + (flashingColor * flashingStage);
                // _spriteRenderer.color = Color.red;
                if (flashingStage != 0)
                {
                    // You are not expected to understand this
                    spriteRenderer.color = (basicColor * (1 - (flashingStage / 100))) + (flashingColor * (flashingStage / 100));
                }
                else
                {
                    spriteRenderer.color = basicColor;
                }
            }
        }

        public void Flash()
        {
            isFlashing = true;
            flashingBackwards = false;
            flashingStage = 0;
        }

        public void OnCollisionStay2D(Collision2D col)
        {
            if (col.gameObject.layer == (int)Layers.Player)
            {
                col.gameObject.GetComponent<IDamageable>().ReceiveDamage(
                    damage, transform.position, knockbackForce);
            }
        }
        
        protected void ChangeLookingDirection()
        {
            lookingDirection *= -1;
            gameObject.transform.localScale = new Vector3(Mathf.Abs(gameObject.transform.localScale.x) * lookingDirection, gameObject.transform.localScale.y, 1);
        }

        // Knockback implementation is up to child classes 
        public virtual void ReceiveDamage(int damage, Vector2 sourcePoint = default, int knockbackForce=0)
        {
            HP -= damage;
            Flash();

            if (HP <= 0)
            {
                if(deathEffect)
                    Destroy(GameObject.Instantiate(deathEffect, transform.position, Quaternion.identity), 2.0f);
                Destroy(gameObject);
            }
            enemyUI.UpdateHP(HP);
        }
    }
}
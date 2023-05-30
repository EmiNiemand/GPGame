using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>
 *      This script supports playing sound (with AudioSource
 *      on GameObject), replaces normal sprite with after damage
 *      sprite and activates particle system that's added as one
 *      of GameObject's children (but can work without either).
 *      Persistent and TimeToDestroy control whether GameObject
 *      gets destroyed/removed after taking damage (and how long
 *      it takes for it to disappear)
 * </summary>
 * <remarks>
 *      Mandatory components are <b>BoxCollider2D</b> and
 *      <b>SpriteRenderer</b> attached to GameObject.
 * </remarks>
 */
public class OneHitFoliage : MonoBehaviour, IDamageable
{
    // Optional components
    public Sprite afterDamage;
    public bool persistent = false;
    public float timeToDestroy = 5.0f;
    public bool playShrinkAnimation = true;
    private ParticleSystem destructParticles;
    private AudioSource audioSource;
    // Mandatory components
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        if(!spriteRenderer || !boxCollider)
            Debug.LogError("[OneHitFoliage] Missing mandatory components here!", transform);
        
        destructParticles = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ReceiveDamage(int damage, Vector2 sourcePoint = default, int knockbackForce = 0)
    {
        boxCollider.enabled = false;
        
        if (afterDamage) spriteRenderer.sprite = afterDamage;
        else spriteRenderer.enabled = false;
        
        if(destructParticles) destructParticles.Play();
        if(audioSource) audioSource.Play();
        if (playShrinkAnimation) StartCoroutine(ShrinkAndGrow());
        
        if(!persistent) Destroy(gameObject, timeToDestroy);
    }

    private IEnumerator ShrinkAndGrow()
    {
        var scale = transform.localScale; 
        transform.localScale = new Vector3(scale.x, scale.y * 0.8f, scale.z);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = new Vector3(scale.x, scale.y, scale.z);
    }
}

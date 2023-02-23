using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
    public Sprite ActiveSprite;
    public Sprite InactiveSprite;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        if (_audioSource)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
                gameObject.GetComponent<SpriteRenderer>().sprite = InactiveSprite;
            }
            else
            {
                _audioSource.Play();
                gameObject.GetComponent<SpriteRenderer>().sprite = ActiveSprite;
            }
        }
    }
}

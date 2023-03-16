using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowWormTriggers : MonoBehaviour, IUsable
{
    private GlowWorm glowworm;
    [SerializeField] private bool stoppingBlock;
    [SerializeField] private GameObject tutorialText;
        
    // Start is called before the first frame update
    void Start()
    {
        glowworm = FindObjectOfType<GlowWorm>();
        tutorialText.SetActive(false);
    }

    public void OnEnter(GameObject user)
    {
        glowworm.SetStopped(stoppingBlock);
        tutorialText.SetActive(stoppingBlock);
        Destroy(gameObject);
    }

    public void OnExit(GameObject user) { }
    public void Use(GameObject user) { }
}

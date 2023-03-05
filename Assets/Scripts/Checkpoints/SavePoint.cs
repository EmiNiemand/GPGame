using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace CheckPoints
{
    public class SavePoint : MonoBehaviour, IUsable
    {
        [SerializeField] private int healValue;
        [SerializeField] private float cooldown;
        
        private GameObject player;
        private ParticleSystem particleSystem;
        private GameObject activationIndicator;
        
        private bool bIsUsing = false;
        private bool bIsOnCooldown = false;

        // Start is called before the first frame update
        void Start()
        {
           particleSystem = GetComponentInChildren<ParticleSystem>();
           activationIndicator = transform.Find("ActivationIndicator").gameObject;
        }
        
        public void OnEnter(GameObject user)
        {
            player = user;
            activationIndicator.SetActive(true);
        }

        public void OnExit(GameObject user)
        {
            player = null;
            activationIndicator.SetActive(false);
        }

        public void Use(GameObject user)
        {
            if (bIsOnCooldown) return;
            StartCoroutine(Heal());
        }
        
        private IEnumerator Heal()
        {
            bIsOnCooldown = true;
            activationIndicator.SetActive(false);
            player.GetComponent<Player.PlayerManager>().Heal(healValue);
            FindObjectOfType<GameManager>().savePointEvent.Invoke(gameObject);
            particleSystem.Play();
            
            yield return new WaitForSeconds(cooldown);
            
            bIsOnCooldown = false;
            player = null;
        }

        public void Activate()
        {
            Light2D[] lights = GetComponentsInChildren<Light2D>();
            foreach (var singleLight in lights)
            {
                singleLight.enabled = true;
            }
        }

        public void Deactivate()
        {
            Light2D[] lights = GetComponentsInChildren<Light2D>();
            foreach (var singleLight in lights)
            {
                singleLight.enabled = false;
            }
        }
    }
}

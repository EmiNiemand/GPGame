using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace CheckPoints
{
    public class SavePoint : MonoBehaviour
    {
        [HideInInspector] public UnityEvent ActivateEvent;
        [HideInInspector] public UnityEvent DeactivateEvent;
        
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

           if (ActivateEvent == null) ActivateEvent = new UnityEvent();
           ActivateEvent.AddListener(Activate);
           
           if (DeactivateEvent == null) DeactivateEvent = new UnityEvent();
           DeactivateEvent.AddListener(Deactivate);
        }

        // Update is called once per frame
        void Update()
        {
            if (player == null) return;

            bIsUsing = player.GetComponent<Player.PlayerInteraction>().IsUsing();

            if (bIsUsing && !bIsOnCooldown)
            {
                StartCoroutine(Heal());
            }
            activationIndicator.SetActive(true);
        }

        private IEnumerator Heal()
        {
            bIsOnCooldown = true;
            player.GetComponent<Player.PlayerCombat>().Heal(healValue);
            FindObjectOfType<GameManager>().savePointEvent.Invoke(gameObject);
            particleSystem.Play();
            yield return new WaitForSeconds(cooldown);
            bIsOnCooldown = false;
            player = null;
            activationIndicator.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider != null && collider.CompareTag("Player"))
            {
                player = collider.gameObject.transform.parent.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider != null && collider.CompareTag("Player"))
            {
                player = null;
            }
        }

        private void Activate()
        {
            Light2D[] lights = GetComponentsInChildren<Light2D>();
            foreach (var singleLight in lights)
            {
                singleLight.enabled = true;
            }
        }

        private void Deactivate()
        {
            Light2D[] lights = GetComponentsInChildren<Light2D>();
            foreach (var singleLight in lights)
            {
                singleLight.enabled = false;
            }
        }
    }
}

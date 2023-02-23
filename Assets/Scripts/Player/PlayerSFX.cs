using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public enum PlayType {looping, oneShot, onEvent};
    
    [System.Serializable]
    public class StateAudioClip
    {
        public PlayerStates state;
        public AudioClipSettings clipSettings;
    
    }
    
    [System.Serializable]
    public class AudioClipSettings
    {
        public AudioClip clip;
        public PlayType playType = PlayType.looping;
    }
    
    
    public class PlayerSFX : MonoBehaviour
    {
        private PlayerMovement playerMovement;
        private PlayerStates currentState;
        private AudioSource audioSource;
        [SerializeField] private List<StateAudioClip> playerSoundsList;
        private Dictionary<PlayerStates, AudioClipSettings> playerSounds;
        private bool bIsPlaying = false;
    
        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            playerMovement = GetComponent<PlayerMovement>();
            currentState = PlayerStates.Idle;
    
            playerSounds = new Dictionary<PlayerStates, AudioClipSettings>();
    
            foreach (var kvp in playerSoundsList)
            {
                playerSounds.Add(kvp.state, kvp.clipSettings);
            }
        }
    
        // Update is called once per frame
        void Update()
        {
            if(currentState != playerMovement.GetPlayerState()) {
                currentState = playerMovement.GetPlayerState();
                audioSource.Stop();
                var stateAudio = playerSounds[currentState];
                audioSource.clip = stateAudio.clip;
                audioSource.loop = (stateAudio.playType == PlayType.looping);
                if(stateAudio.playType != PlayType.onEvent) audioSource.Play();
            }
        }
    
        public void OnStep() {
            audioSource.Stop();
            audioSource.Play();
        }
    }
}

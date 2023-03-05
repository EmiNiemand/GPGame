using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public enum PlayType {Looping, OneShot, OnEvent};
    
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
        public PlayType playType = PlayType.Looping;
    }
    
    
    public class PlayerSFX : MonoBehaviour
    {
        private PlayerStates currentState;
        private AudioSource audioSource;
        [SerializeField] private List<StateAudioClip> playerSoundsList;
        private Dictionary<PlayerStates, AudioClipSettings> playerSounds;
        private bool bIsPlaying = false;
    
        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            currentState = PlayerStates.Idle;
    
            playerSounds = new Dictionary<PlayerStates, AudioClipSettings>();
    
            foreach (var kvp in playerSoundsList)
            {
                playerSounds.Add(kvp.state, kvp.clipSettings);
            }
        }

        public void UpdateState(PlayerStates newState)
        {
            currentState = newState;
            audioSource.Stop();
            var stateAudio = playerSounds[currentState];
            audioSource.clip = stateAudio.clip;
            audioSource.loop = (stateAudio.playType == PlayType.Looping);
            if(stateAudio.playType != PlayType.OnEvent) audioSource.Play();
        }
    
        public void OnStep() {
            Debug.Log("STEP");
            audioSource.Stop();
            audioSource.Play();
        }
    }
}

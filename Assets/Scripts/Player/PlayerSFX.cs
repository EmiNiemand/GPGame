using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public enum PlayType {Looping, OneShot, OnEvent};

    [System.Serializable]
    public class AudioClipSettings
    {
        public AudioClip clip;
        public PlayType playType = PlayType.Looping;
    }

    [System.Serializable]
    public class StateAudioClip
    {
        public PlayerStates state;
        public AudioClipSettings clipSettings;
    }
    
    // If you want to add some new Combat sound, add it here
    public enum CombatSoundType { Swing, Hit, Hurt }

    [System.Serializable]
    public class CombatAudioClip
    {
        public CombatSoundType type;
        public AudioClipSettings clipSettings;
    }

    public class PlayerSFX : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private List<StateAudioClip> playerSoundsList;
        private Dictionary<PlayerStates, AudioClipSettings> playerSounds;
        
        [Header("Combat")]
        [SerializeField] private List<CombatAudioClip> playerCombatSoundsList;
        private Dictionary<CombatSoundType, AudioClipSettings> playerCombatSounds;
        
        private PlayerStates currentState;
        
        // Last two audio sources are reserved for Combat sounds
        // (it has to be done in this way because combat and movement
        // sounds can and will happen at the same time)
        private AudioSource[] audioSource;
        private int sourceIndex;
        private int combatIndex;
        private readonly int combatBufferSize = 2;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = transform.Find("PlayerAudioSources").GetComponentsInChildren<AudioSource>();
            sourceIndex = 0;
            combatIndex = 0;
            
            currentState = PlayerStates.Idle;
    
            // Convert player sounds from list to dictionary
            // ---------------------------------------------
            playerSounds = new Dictionary<PlayerStates, AudioClipSettings>();
            playerSoundsList.ForEach(item => 
                playerSounds.Add(item.state, item.clipSettings));
            
            
            // Same but for Combat sounds
            // --------------------------
            playerCombatSounds = new Dictionary<CombatSoundType, AudioClipSettings>();
            playerCombatSoundsList.ForEach(item => 
                playerCombatSounds.Add(item.type, item.clipSettings));
        }

        public void UpdateState(PlayerStates newState)
        {
            if(audioSource.Length == 0) return; 
            
            // Stop loops at previous source
            // -----------------------------
            if(audioSource[sourceIndex].loop)
                audioSource[sourceIndex].Stop();
            
            // Update index to the next audio source
            // -------------------------------------
            sourceIndex++;
            sourceIndex %= audioSource.Length - combatBufferSize - 1;

            // Set up audio
            // ------------
            currentState = newState;
            audioSource[sourceIndex].Stop();
            var stateAudio = playerSounds[currentState];
            audioSource[sourceIndex].clip = stateAudio.clip;
            audioSource[sourceIndex].loop = (stateAudio.playType is PlayType.Looping);
            
            // Play immediately if audio is of Looping or OneTime type
            // -------------------------------------------------------
            if(stateAudio.playType != PlayType.OnEvent) audioSource[sourceIndex].Play();
        }

        public void PlayCombatSound(CombatSoundType type)
        {
            int offsetIndex = combatBufferSize + combatIndex;
            // Stop loops at previous source
            // -----------------------------
            if(audioSource[^offsetIndex].loop)
                audioSource[^offsetIndex].Stop();
            
            // Update index to the next audio source
            // -------------------------------------
            combatIndex = ++combatIndex % combatBufferSize;
            offsetIndex = combatBufferSize + combatIndex;
            
            // Set up audio
            // ------------
            audioSource[offsetIndex].Stop();
            var combatSound = playerCombatSounds[type];
            audioSource[offsetIndex].clip = combatSound.clip;
            audioSource[offsetIndex].loop = (combatSound.playType is PlayType.Looping);
            
            // Play immediately if audio is of Looping or OneTime type
            // -------------------------------------------------------
            if(combatSound.playType != PlayType.OnEvent) audioSource[offsetIndex].Play();
            
            //INFO: right now OnEvent combat sounds are not supported, but ready to implement
        }
    
        public void OnStep() {
            sourceIndex++;
            sourceIndex %= audioSource.Length - combatBufferSize - 1;
            
            var stateAudio = playerSounds[currentState];
            audioSource[sourceIndex].clip = stateAudio.clip;
            audioSource[sourceIndex].loop = false;
            
            audioSource[sourceIndex].Play();
        }
    }
}

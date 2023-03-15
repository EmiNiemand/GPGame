using System;
using UnityEngine;

namespace Player
{
    public enum CameraSingularEffect { Shake }
    public enum CameraPersistentEffect { LookDown, LookUp }
    
    public class PlayerCameraEffects : MonoBehaviour
    {
        private Animator animator;
        private CameraPersistentEffect lastEffect;

        private void Start()
        {
            animator = Camera.main.GetComponent<Animator>();
        }

        public void PlayEffect(CameraSingularEffect type)
        {
            animator.SetTrigger(type.ToString());
        }

        public void SetPersistentEffect(CameraPersistentEffect type, bool activated = true)
        {
            animator.SetBool(type.ToString(), activated);
        }
    }
}
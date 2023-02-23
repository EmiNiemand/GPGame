using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        private bool bIsUsePressed;
        public UnityEvent onUsePressed; 

        public void OnUse(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                bIsUsePressed = true;
                onUsePressed.Invoke();
            }
            else if (context.canceled)
            {
                bIsUsePressed = false;
            }
        }

        public bool IsUsing()
        {
            return bIsUsePressed;
        }
    }
}


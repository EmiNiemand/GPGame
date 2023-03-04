using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject pauseFirstButton;

        [HideInInspector] public UnityEvent pauseUnpause;
        
        private void Start()
        {
            pauseUnpause.AddListener(PauseUnpause);
        }
        
        public void OnPauseUnpause(InputAction.CallbackContext context)
        {
            if (context.started) PauseUnpause();
        }

        private void PauseUnpause()
        {
            if (!pauseMenu.activeInHierarchy)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(pauseFirstButton);
            }
            else
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        public void OnClickContinue()
        {
            PauseUnpause();
        }
        
        public void OnClickReturn()
        {
            SceneManager.LoadScene("Scenes/MainMenu");
        }
        
        public void OnClickQuit()
        {
            Application.Quit();
        }
    }
}

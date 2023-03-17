using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject pauseFirstButton;
        public bool activated = false;

        public bool PauseUnpause()
        {
            activated = !activated;
            pauseMenu.SetActive(activated);
            
            if (activated)
            {
                Time.timeScale = 0f;
            
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(pauseFirstButton);
                return true;
            }
            
            Time.timeScale = 1f;
            return false;
        }

        public void OnClickContinue()
        {
            // PauseUnpause();
            //TODO: improve, this will do as workaround for now
            FindObjectOfType<PlayerManager>().PauseGame();
        }
        
        public void OnClickReturn()
        {
            SceneManager.LoadScene("Scenes/MainMenu");
            Time.timeScale = 1f;
        }
        
        public void OnClickQuit()
        {
            Application.Quit();
        }
    }
}

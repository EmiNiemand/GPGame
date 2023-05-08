using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject optionsMenu;
        public GameObject levelSelectMenu;
        
        public void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnClickPlayButton() { SceneManager.LoadScene("Scenes/GameScene"); }
        public void OnClickOptionsButton() {  }
        public void OnClickExitButton() { Application.Quit(); }
    }
}

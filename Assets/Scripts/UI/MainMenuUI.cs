using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public Slider levelSlider;
        public TextMeshProUGUI levelTitle;
        
        public void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnClickPlayButton() { SceneManager.LoadScene(1); }
        public void OnClickOptionsButton() {  }
        public void OnClickExitButton() { Application.Quit(); }

        public void OnLevelSliderChange()
        {
            levelTitle.text = Utils.UI_LevelNames[(int)levelSlider.value];
        }
    }
}

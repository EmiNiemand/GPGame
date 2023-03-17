using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    //TODO: maybe improve later
    // public enum PlayerAction {Move, Dash, Jump, WallJump, Use, }
    public List<TextMeshProUGUI> textKeyboard;
    public List<TextMeshProUGUI> textPad;

    public void SetControlScheme(string scheme)
    {
        if (scheme.Equals("Keyboard"))
        {
            textKeyboard.ForEach(item => item.color = Color.white);
            textPad.ForEach(item => item.color = Color.clear);
        }
        else
        {
            textPad.ForEach(item => item.color = Color.white);
            textKeyboard.ForEach(item => item.color = Color.clear);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialoguePassive : MonoBehaviour
{
    private TextMeshProUGUI dialogueText;
    private GameObject dialogueBackdrop;

    // Start is called before the first frame update
    public void Setup()
    {
        dialogueText = transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        dialogueBackdrop = transform.Find("DialogueBackdrop").gameObject;
        HideDialogue();
    }

    public void ShowDialogue(string newText)
    {
        dialogueText.color = Color.black;
        dialogueText.text = newText;
        dialogueBackdrop.SetActive(true);
    }

    public void HideDialogue()
    {
        dialogueText.color = Color.clear;
        dialogueBackdrop.SetActive(false);
    }
}

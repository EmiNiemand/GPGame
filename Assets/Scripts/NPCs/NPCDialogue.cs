using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : NPCBase
{
    [SerializeField] protected NPCDialogueScriptable npcDialogueInfo;
    protected DialoguePassive dialogue;
    
    int dialogueLineCounter;

    void Start()
	{
        npcInfo = npcDialogueInfo;
        base.Setup();

        dialogueLineCounter = 0;

        dialogue = transform.Find("NPCInterface").GetComponent<DialoguePassive>();
        dialogue.Setup();
	}
	public override void OnInteract() {
		if(dialogueLineCounter == npcDialogueInfo.dialogueLines.Count)
		{
			dialogueLineCounter = 0;
			dialogue.HideDialogue();

			npcUI.SetName(npcDialogueInfo.characterName);
			npcUI.SetDescription(npcDialogueInfo.description);
			return;
		}
        dialogue.ShowDialogue(npcDialogueInfo.dialogueLines[dialogueLineCounter]);
        dialogueLineCounter++;
    }
}

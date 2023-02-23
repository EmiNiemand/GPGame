using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "NPCs/Dialogue")]
public class NPCDialogueScriptable : NPCScriptable
{
    [TextArea]
    public List<string> dialogueLines;
}

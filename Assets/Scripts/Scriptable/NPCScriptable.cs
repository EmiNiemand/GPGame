using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScriptable : ScriptableObject
{
    public string characterName;
    public string initialName = "???";
    [TextArea]
    public string description;
    [TextArea]
    public string initialDescription;
}

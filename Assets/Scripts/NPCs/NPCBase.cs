using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCBase : MonoBehaviour
{
	protected NPCScriptable npcInfo;
	protected NPC_UI npcUI;
	
	protected void Setup()
	{
		npcUI = transform.Find("NPCInterface").GetComponent<NPC_UI>();
		npcUI.Setup();
		npcUI.SetName(npcInfo.initialName);
		npcUI.SetDescription(npcInfo.initialDescription);
		npcUI.SetDescriptionActive(false);
	}
	public virtual void OnBoundaryEntry() { npcUI.SetDescriptionActive(true); }
	public virtual void OnBoundaryExit() { npcUI.SetDescriptionActive(false); }
	public virtual void OnInteract() { }
}

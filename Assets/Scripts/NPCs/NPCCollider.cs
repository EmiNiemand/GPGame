using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCollider : MonoBehaviour, IUsable
{
	private NPCBase npc;
	private bool playerTriggered;
	// Start is called before the first frame update
	void Start()
	{
		npc = transform.parent.GetComponent<NPCBase>();
	}

	// IDamageable interface implementation
	// ------------------------------------
    public void OnEnter(GameObject user) { npc.OnBoundaryEntry(); }
    public void OnExit(GameObject user) { npc.OnBoundaryExit(); }
    public void Use(GameObject user) { npc.OnInteract(); }
}

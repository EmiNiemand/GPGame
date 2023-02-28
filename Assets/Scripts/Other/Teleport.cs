using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IUsable
{
    private Vector2 destination;

    // Start is called before the first frame update
    void Start()
    {
        destination = transform.Find("Destination").position;
    }

	public void OnEnter(GameObject user)
	{
        user.transform.position = destination;
	}

	public void OnExit(GameObject user) { }
	public void Use(GameObject user) { }
}

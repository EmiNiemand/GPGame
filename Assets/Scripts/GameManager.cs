using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent<GameObject> respawnPointEvent;
    [HideInInspector] public UnityEvent<GameObject> savePointEvent;
    [HideInInspector] public UnityEvent respawnEvent;
    
    private GameObject respawnPoint;
    private GameObject savePoint;

    private GameObject player;
    
    // TODO: temp when save implemented it supposed to be changed(deleted)
    [SerializeField] private GameObject initSavePoint;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        savePoint = initSavePoint;
        respawnPoint = savePoint;
        
        if (respawnPointEvent == null) respawnPointEvent = new UnityEvent<GameObject>();
        respawnPointEvent.AddListener(SetRespawnPoint);
        if (savePointEvent == null) savePointEvent = new UnityEvent<GameObject>();
        savePointEvent.AddListener(SetSavePoint);
        if (respawnEvent == null) respawnEvent = new UnityEvent();
        respawnEvent.AddListener(Respawn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetRespawnPoint(GameObject newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
    
    private void SetSavePoint(GameObject newSavePoint)
    {
        //TODO: add game saving here
        if (savePoint.GetComponent<CheckPoints.SavePoint>() != null)
        {
            savePoint.GetComponent<CheckPoints.SavePoint>().DeactivateEvent.Invoke();
        }
        savePoint = newSavePoint;
        FindObjectOfType<EnemiesManager>().respawnEnemiesEvent.Invoke();
        savePoint.GetComponent<CheckPoints.SavePoint>().ActivateEvent.Invoke();
        respawnPoint = newSavePoint;
    }

    private void Respawn()
    {
        FindObjectOfType<EnemiesManager>().respawnEnemiesEvent.Invoke();
        player.transform.position = respawnPoint.transform.position;
    }
}

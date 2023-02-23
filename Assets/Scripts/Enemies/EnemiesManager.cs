using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies
{
    public class EnemiesManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent respawnEnemiesEvent;
    
        // Start is called before the first frame update
        void Start()
        {
            if (respawnEnemiesEvent == null) respawnEnemiesEvent = new UnityEvent();
            respawnEnemiesEvent.AddListener(RespawnEnemies);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void RespawnEnemies()
        {
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

            foreach (var spawner in spawners)
            {
                spawner.GetComponent<EnemySpawner>().respawnEnemyEvent.Invoke();
            }
        }
    }
}

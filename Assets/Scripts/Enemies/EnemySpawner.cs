using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [HideInInspector] public UnityEvent respawnEnemyEvent;
    
        [SerializeField] private bool bRespawn = false;
        [SerializeField] private GameObject enemyPrefab;
        [Tooltip("1 - right, -1 - left")] public int lookingDirection = 1;
    
        private GameObject enemySpawned;
    
        // Start is called before the first frame update
        void Start()
        {
            if (respawnEnemyEvent == null) respawnEnemyEvent = new UnityEvent();
            respawnEnemyEvent.AddListener(RespawnEnemy);

            enemySpawned = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            enemySpawned.transform.SetParent(gameObject.transform);
        }
    
        private void RespawnEnemy()
        {
            if (!bRespawn) return;
            if (enemySpawned != null) Destroy(enemySpawned);
            
            enemySpawned = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            enemySpawned.transform.SetParent(gameObject.transform);
        }
    }
}

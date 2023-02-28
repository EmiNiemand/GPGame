using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Plant : Enemy
    {
        private GameObject missile;
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private float distanceToActivate;
        private GameObject player;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            player = GameObject.FindWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if (Mathf.Abs(Vector2.Distance(player.transform.position, transform.position)) > distanceToActivate) return;
            if (missile) return;
            missile = Instantiate(missilePrefab, transform.position + 
                                                 new Vector3(spriteRenderer.size.x / 2 * lookingDirection, spriteRenderer.size.y / 4, 0), Quaternion.identity);
            missile.transform.SetParent(gameObject.transform);
        }
    }
}


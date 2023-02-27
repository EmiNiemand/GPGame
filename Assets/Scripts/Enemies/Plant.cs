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

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (missile != null) return;
            if (other && other.CompareTag("Player"))
            {
                missile = Instantiate(missilePrefab, transform.position + 
                                                     new Vector3(spriteRenderer.size.x / 2 * lookingDirection, spriteRenderer.size.y / 4, 0), Quaternion.identity);
                missile.transform.SetParent(gameObject.transform);
            }
        }
    }
}


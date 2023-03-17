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
        [SerializeField] private Vector3 missileSpawnShift = Vector3.zero;
        
        private GameObject player;
        private bool animatingAttack = false;

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
            if (missile || animatingAttack) return;
            
            animatingAttack = true;
            animator.SetTrigger("Attack");
        }

        void AE_ShootBullet()
        {
            missile = Instantiate(missilePrefab, transform.position + new Vector3(missileSpawnShift.x * lookingDirection, 
                missileSpawnShift.y, missileSpawnShift.z), Quaternion.identity);
            missile.transform.SetParent(gameObject.transform);
            animatingAttack = false;
        }
    }
}


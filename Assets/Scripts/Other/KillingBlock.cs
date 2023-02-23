using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    public class KillingBlock : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider != null && collider.CompareTag("Player"))
            {
                collider.GetComponent<IDamageable>().ReceiveDamage(10000);
            }
        }
    }
}


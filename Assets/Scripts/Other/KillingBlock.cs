using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    public class KillingBlock : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider != null && collider.CompareTag("Player"))
            {
                collider.gameObject.GetComponentInParent<IDamageable>().ReceiveDamage(10000);
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Layers
{
    Environment=6, Damageable, Usable, Player, PlayerWeapon
}

public class Utils : MonoBehaviour
{
    public const int KnockbackMult = 100;

    public static bool ShootBoxcast(Vector2 rayInitPos, Vector2 size, Vector2 direction, float distance, string hitTag)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(rayInitPos, size, 0, direction, distance);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag(hitTag))
            {
                return true;
            }
        }

        return false;
    }

    public static bool ShootRaycast(Vector2 rayInitPos, Vector2 direction, float distance, string hitTag)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayInitPos, direction, distance);
        
        Debug.DrawRay(rayInitPos, direction * distance, Color.red, 0.3f);
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag(hitTag))
            {
                return true;
            }
        }

        return false;
    }
}

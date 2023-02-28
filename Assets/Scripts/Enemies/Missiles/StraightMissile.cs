using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.Missiles
{
    public class StraightMissile : Missile
    {
        private void FixedUpdate()
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(GetComponentInParent<Enemy>().lookingDirection * speed, 0), ForceMode2D.Impulse);
        }
    }
}

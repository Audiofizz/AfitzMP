using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DamageZone : MonoBehaviour, Damageable
{
    public Player damageParrent;

    public bool TakeDamage(float amount, int fromClient)
    {
        return damageParrent.TakeDamage(amount, fromClient);
    }
}

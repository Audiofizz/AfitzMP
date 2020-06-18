using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameServer
{
    class Damages : MonoBehaviour
    {
        float damage = 50;

        List<Damageable> alreadyDamaged = new List<Damageable>();

        private void OnTriggerEnter(Collider other)
        {
            /*            Debug.Log($"Object {name} got hit by {other.name}");*/
            Damageable Object = other.gameObject.GetComponent<Damageable>();
            if (Object != null)
            {
                if (!alreadyDamaged.Contains(Object))
                {
                    Object.TakeDamage(damage, -1);
                    alreadyDamaged.Add(Object);
                }
            }
        }
    }
}

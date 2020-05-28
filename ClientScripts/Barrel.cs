using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour, ExplodeOnDeath
{
    public void Explode()
    {
        GameObject temp = Instantiate(Prefabs.instance.DeathParticals[0], transform.position, Quaternion.identity);
        Destroy(temp, 2);
    }
}

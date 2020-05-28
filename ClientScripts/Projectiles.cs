using System;
using System.Collections.Generic;
using UnityEngine;


class Projectiles
{
    public static void SpawnProjectile(int _upid, Vector3 _vec1, Vector3 _vec2)
    {
        GameObject temp;
        switch (_upid)
        {
            case 0:
                Vector3 HitDir = _vec2 - _vec1;
                temp = UnityEngine.Object.Instantiate(Prefabs.instance.Effects[0], _vec1, Quaternion.LookRotation(HitDir, Vector3.up));
                temp.transform.localScale = new Vector3(1, 1, HitDir.magnitude);
                UnityEngine.Object.Destroy(temp, .5f);
                break;
            case 1:
                temp = UnityEngine.Object.Instantiate(Prefabs.instance.Effects[1], _vec2, Quaternion.LookRotation(Vector3.forward, Vector3.up));
                UnityEngine.Object.Destroy(temp, 1f);
                break;

        }
    }
}


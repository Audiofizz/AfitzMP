using System;
using UnityEngine;

namespace GameServer
{
    class Explosion : MonoBehaviour
    {
        public Vector3 size = Vector3.one;

        float lerp = .05f;

        public void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, size, lerp);

            if (transform.localScale.x >= size.x - lerp)
                Destroy(gameObject);
        }
    }
}

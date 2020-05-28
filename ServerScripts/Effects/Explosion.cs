using System;
using UnityEngine;

namespace GameServer
{
    class Explosion : MonoBehaviour
    {
        public float size = 1;

        float lerp = .05f;

        public void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * size, lerp);

            if (transform.localScale.x >= size - lerp)
                Destroy(gameObject);
        }
    }
}

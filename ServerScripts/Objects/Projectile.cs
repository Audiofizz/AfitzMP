using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    class Projectile : PhysicsObject
    {
        public static int itemIndex = 0;

        public float speed = 50f / Constants.TICKS_PER_SEC;

        public float damage = 10f;

        int lifeTime = 5;

        public Projectile()
        {
            SetMass(10);
        }

        public void Initialize(Quaternion _rotation, Vector3 _pos, int _owner, int _inputTick)
        {
            id = itemIndex;
            itemIndex++;

            Destroy(gameObject, lifeTime);

            transform.rotation = _rotation;
            owner = _owner;

            veclocity += transform.forward * speed;

            transform.position = _pos - veclocity;

            createdTick = Time.instance.tick - _inputTick;

            ServerSend.SpawnItem(this);
        }

        private void Update()
        {
            /*if (isGrounded)
            {
                death = true;
            }*/

            if (veclocity.magnitude <= .01f || death)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnDestroy()
        {
            //Debug.Log($"Sending Destroy for Item: {id}");
            ServerSend.DestroyProjectile(id);
        }

        private void OnTriggerEnter(Collider other)
        {
            /*            Debug.Log($"Object {name} got hit by {other.name}");*/
            Damageable Object = other.gameObject.GetComponent<Damageable>();
            if (Object != null)
            { 
                
                if (Object.TakeDamage(damage,owner))
                    Destroy(this.gameObject);
            }
        }

        /*private void isVisable(int VisableToID)
        {
            ServerSend.SpawnItem(VisableToID,this);
        }*/
    }
}

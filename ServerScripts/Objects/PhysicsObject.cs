using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Drawing;

namespace GameServer
{
    class PhysicsObject : ObjectData
    {

        [Header("Components")]

        public CharacterController cc;

        [HideInInspector] public bool isGrounded = false;

        private float gravity = -2.5f / Constants.TICKS_PER_SEC;

        private float mass = 1 / Constants.MASS_MODIFIER;

        [HideInInspector] public Vector3 veclocity = Vector3.zero;

        private float thickness = 0.2f; //Old Plane Colision

        public void SetMass(float _mass)
        {
            mass = _mass / Constants.MASS_MODIFIER;
        }

        public float GetMass()
        {
            return mass * Constants.MASS_MODIFIER;
        }

        private void FixedUpdate()
        {
            Phyisics();
        }

        public void Phyisics()
        {
            if (!isGrounded)
            {
                ApplyGravity(1);
            }

            cc.Move(veclocity);
        }

        private void CheckPlaneColision() {
            foreach (Plane plane in GameLogic.Planes.Values)
            {
                if (plane.OnPlane(transform.position + veclocity, thickness) && plane.tag != "")
                {
                    if (plane.HitPlane(transform.position.y, veclocity.y))
                    {
                        if (plane.tag == "Ground")
                        {
                            transform.position = new Vector3(transform.position.x,plane.position.y,transform.position.z);
                            veclocity = new Vector3(veclocity.x, 0, veclocity.z);
                            //isGrounded = true;
                        }
                        if (plane.tag == "Death")
                        {
                            ResetObject();
                        }
                    }
                }
            }
        }

        private void CheckStillOnPlane()
        {
            foreach (Plane plane in GameLogic.Planes.Values)
            {
                if (transform.position.y == plane.position.y)
                {
                    //isGrounded = plane.OnPlane(transform.position + veclocity, thickness);
                }
            }
        }

        public void ApplyGravity(float modifier)
        {
            veclocity.y += (gravity * mass)* modifier;
        }

        public void SetRotation(Quaternion _rotation)
        {
            transform.rotation = _rotation;
        }

        public void ResetObject()
        {
            veclocity = Vector3.zero;
            death = true;
            OnReset();
        }

        public virtual void OnReset()
        {

        }

        public void CheckVisable(Action<int> Callback)
        {
            foreach (Client client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    if (Vector3.Dot(client.player.forward, Vector3.Normalize(transform.position - client.player.transform.position)) > 0.525321989f)
                        Callback(client.player.id);
                }
            }
        }
    } //End class
}

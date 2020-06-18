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

        public PlayerMovement playerMovement;

        private float gravity = -2.5f / Constants.TICKS_PER_SEC;

        private float mass = 1 / Constants.MASS_MODIFIER;

        [HideInInspector] public float mu = 1;

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
            if (!playerMovement.isGrounded)
            {
                ApplyGravity(1);
            }

            playerMovement.MoveDirect(veclocity);
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
                    if (Vector3.Dot(client.player.playerMovement.GetForward(), Vector3.Normalize(transform.position - client.player.transform.position)) > 0.525321989f)
                        Callback(client.player.id);
                }
            }
        }
    } //End class
}

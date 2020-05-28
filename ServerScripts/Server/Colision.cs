using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameServer;

public class Colision : MonoBehaviour
{
    /*[SerializeField] private PhysicsObject obj;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{obj.name} Colided With {collision.gameObject.name}");
        obj.isGrounded = true;
        obj.veclocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            float Rad = 10;
            Debug.Log($"{name} colides with {other.name}");
            Vector3 ndist = transform.position - other.ClosestPointOnBounds(transform.position);
            Vector3 normal = ndist + ndist.normalized * (obj.moveVector.magnitude/ obj.GetMoveSpeed());
            Debug.DrawRay(other.ClosestPointOnBounds(transform.position),normal, Color.red,10f);
            //veclocity = Vector3.Cross(Vector3.Cross(veclocity, normal), normal);
            obj.transform.position += normal;
        }
    }*/
    
}

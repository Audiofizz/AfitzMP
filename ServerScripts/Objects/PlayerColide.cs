using UnityEngine;
using System.Collections;
using GameServer;

public class PlayerColide : MonoBehaviour
{
    private float slopeLimit = 0.1f;

    private CharacterController characterController;
    private CapsuleCollider myCollider;
    private PhysicsObject physicsObject;

    public Transform ColisionNormal;

    public Transform Body;

    [HideInInspector] public Vector3 SlopeNormal;

    //initialize values 
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        myCollider = this.GetComponent<CapsuleCollider>();
        physicsObject = this.GetComponent<PhysicsObject>();
        slopeLimit = Mathf.Sin((90-characterController.slopeLimit)*Mathf.PI/180);
    }

    private void FixedUpdate()
    {
        ColisionNormal.rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross(transform.right, SlopeNormal)), SlopeNormal);

        if (physicsObject.playerMovement.isGrounded)
        {
            if (!Physics.Raycast(transform.position, Vector3.down, .1f))
            {
                //Debug.Log("No Ground");
                physicsObject.veclocity += physicsObject.playerMovement.GetMoveVector()* physicsObject.playerMovement.GetMoveSpeed();
                physicsObject.playerMovement.isGrounded = false;
            }
        }

        if ((!physicsObject.playerMovement.isGrounded || physicsObject.playerMovement.IsSliding()) && physicsObject.playerMovement.GetMoveVector() != Vector3.zero)
        {
            //if (physicsObject.Sliding&&physicsObject.isGrounded) physicsObject.veclocity.y = 0;

            float tempY = physicsObject.veclocity.y;
            physicsObject.veclocity.y = 0;
            float mag = physicsObject.veclocity.magnitude;
            physicsObject.veclocity = ((physicsObject.veclocity*2) + physicsObject.playerMovement.GetMoveVector()).normalized * mag;
            physicsObject.veclocity.y = tempY;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < -slopeLimit)
        {
            physicsObject.veclocity.y = 0;
            return;
        }
        //Debug.Log("COLIDE");

        if (hit.normal.y > slopeLimit)
        {
            physicsObject.playerMovement.isGrounded = true;
            SlopeNormal = hit.normal;
        }
        else
        {
            SlopeNormal = transform.up;
        }

        if (physicsObject.playerMovement.IsSliding())
            physicsObject.veclocity += new Vector3(hit.normal.x, 0, hit.normal.z) * (1f - hit.normal.y);

        physicsObject.veclocity -= (physicsObject.veclocity/2f) * hit.normal.y * physicsObject.mu;

        if (hit.gameObject.tag == "Death")
        {
            physicsObject.ResetObject();
        }
    }

    public void Shrink()
    {
        myCollider.height = 1;
        myCollider.center = new Vector3(0,0.5f,0);
        Body.transform.localScale = new Vector3(1, .5f, 1);
    }

    public void Grow()
    {
        myCollider.height = 2;
        myCollider.center = new Vector3(0, 1, 0);
        Body.transform.localScale = new Vector3(1, 1, 1);
    }
}
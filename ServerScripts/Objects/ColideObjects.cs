using UnityEngine;
using System.Collections;
using GameServer;

public class ColideObjects : MonoBehaviour
{
    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 
    public float slopeLimit = 0.1f;

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private CharacterController characterController;
    private Collider myCollider;
    private PhysicsObject physicsObject;

    //initialize values 
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        myCollider = GetComponent<Collider>();
        physicsObject = GetComponent<PhysicsObject>();
        previousPosition = transform.position + characterController.center;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;

        slopeLimit = Mathf.Sin((90-characterController.slopeLimit)*Mathf.PI/180);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("COLIDE");
        if (hit.normal.y > slopeLimit)
            physicsObject.isGrounded = true;
        else
        {
            physicsObject.veclocity += new Vector3(hit.normal.x,0, hit.normal.z) * hit.normal.y * UnityEngine.Time.deltaTime;
        }

        physicsObject.veclocity -= (physicsObject.veclocity/10f) * hit.normal.y;

        if (hit.gameObject.tag == "Death")
        {
            physicsObject.ResetObject();
        }
    }
}
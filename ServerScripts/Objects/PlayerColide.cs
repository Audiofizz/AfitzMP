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
    
    private Vector3 Accelerate(bool grounded) 
    {
        desiredSpeed = physicsObject.playerMovement.GetMoveSpeed();
        
        var desiredDir = physicsObject.playerMovement.GetMoveVector();
        
        desiredDir[1] = 0;
    
        float currentSpeed = Vector3.Dot(physicsObject.veclocity,desiredDir);
        
        float addspeed = desiredSpeed - currentSpeed;
        
        if (addspeed <= 0)
            return Vector3.zero;
            
        float accelSpeed = desiredSpeed * UnityEngine.Time.deltaTime * grounded?physicsObject.mu:1f;
        
        accelSpeed = Mathf.Min(accelSpeed, addSpeed);
        
        var result = Vector3.zero;
        
        result += desiredDir * accelSpeed;
        
        return result;
    }
    
    private Vector3 Surf(Vector3 normal) 
    {
        var input = physicsObject.playerMovement.GetMoveVector();
        var output = Vector3.zero;
        var backoff = Vector3.Dot(input, normal);

        for (int i = 0; i < 3; i++)
        {
            var change = normal[i] * backoff;
            output[i] = input[i] - change;
        }

        // iterate once to make sure we aren't still moving through the plane
        float adjust = Vector3.Dot(output, normal);
        if (adjust < 0.0f)
        {
            output -= (normal * adjust);
            //		Msg( "Adjustment = %lf\n", adjust );
        }
        
        return output;
    }
    
    private bool CheckGrounded() 
    {
        if (physicsObject.playerMovement.isGrounded) 
        {
            if (!Physics.Raycast(transform.position, Vector3.down, .1f)) {
                physicsObject.playerMovement.isGrounded = false;
                return false;
            }
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        ColisionNormal.rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross(transform.right, SlopeNormal)), SlopeNormal);
        
        bool gounded = CheckGrounded();
        
        if (gounded) {
            physicsObject.veclocity += Accelerate(true);
            Friction(physicsObject.veclocity,physicsObject.mu,0.0027f);
        }

        if ((!gounded /*|| physicsObject.playerMovement.IsSliding()*/) && physicsObject.playerMovement.GetMoveVector() != Vector3.zero)
        {
            physicsObject.veclocity += Accelerate(false);
        
            //if (physicsObject.Sliding&&physicsObject.isGrounded) physicsObject.veclocity.y = 0;
            
            //Disabling to test SurfDrift()
                /*float tempY = physicsObject.veclocity.y;
                physicsObject.veclocity.y = 0;
                float mag = physicsObject.veclocity.magnitude;
                physicsObject.veclocity = ((physicsObject.veclocity*2) + physicsObject.playerMovement.GetMoveVector()).normalized * mag;
                physicsObject.veclocity.y = tempY;*/
        }
        
        if (physicsObject.playerMovement.IsSliding() && gounded) 
        {
            physicsObject.veclocity += Surf(SlopeNormal);
        }
    }
    
    private void ComputeNormal(Vector3 normal)
    {
        if (normal.y < -slopeLimit)
        {
            physicsObject.veclocity.y = 0;
            return;
        }
        //Debug.Log("COLIDE");
        if (normal.y > slopeLimit)
        {
            if (physicsObject.playerMovement.isGrounded) {
                SlopeNormal += normal;
                SlopeNormal.Normalize();
                return;
            }
            
            physicsObject.playerMovement.isGrounded = true;
            SlopeNormal = normal;
        }
        else
        {
            physicsObject.veclocity += Surf(normal);
            SlopeNormal = transform.up;
        }
    }
    
    private void Friction(ref Vector3 velocity, float friction, float stopSpeed)
    {
        var speed = velocity.magnitude;

        if (speed < 0.0001905f)
        {
            return;
        }

        var drop = 0f;

        // apply ground friction
        var control = (speed < stopSpeed) ? stopSpeed : speed;
        drop += control * friction * UnityEngine.Time.deltaTime;

        // scale the velocity
        var newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;

        if (newspeed != speed)
        {
            newspeed /= speed;
            velocity *= newspeed;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ComputeNormal(hit.normal);
        
        //Disabling to test SurfDrift()
            /*if (physicsObject.playerMovement.IsSliding())
                physicsObject.veclocity += new Vector3(hit.normal.x, 0, hit.normal.z) * (1f - hit.normal.y);

            physicsObject.veclocity -= (physicsObject.veclocity/2f) * hit.normal.y * physicsObject.mu;*/

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

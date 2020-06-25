using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameServer;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]

    public PlayerData playerData;

    public PlayerColide objectColider;

    public CharacterController cc;

    [HideInInspector] public bool isGrounded = false;

    private float moveSpeed;

    private bool Sliding = false;

    private float JumpVal = 0;

    private bool canJump = false;
    
    private bool holdJump = false;

    //Change this to wishDir
    private Vector3 moveVector;

    private float jumpHeight = 20f / Constants.TICKS_PER_SEC;

    private Vector3 forward;

    public float GetMoveSpeed() { return moveSpeed; }
    public bool IsSliding() { return Sliding; }
    public Vector3 GetForward() { return forward; }
    public Vector3 GetMoveVector() { return moveVector; }

    public void CalculateValues(bool[] inputs, out Vector3 _inputDir)
    {
        if (inputs[4] != true && isGrounded)
            canJump = true;

        Sliding = ComputeSlide(inputs[8]);

        moveSpeed = CalculateMoveSpeed(inputs[5]);

        _inputDir = CalculateInputVector(inputs);
    }

    private float CalculateMoveSpeed(bool Value)
    {
        if (Value)
            return playerData.baseMoveSpeed * playerData.runModifer;
        return playerData.baseMoveSpeed;
    }

    private bool ComputeSlide(bool Value)
    {
        if (Value)
            objectColider.Shrink();
        else
            objectColider.Grow();

        return Value;
    }

    private Vector3 CalculateInputVector(bool[] inputs)
    {
        Vector3 _inputDir = Vector3.zero;
        _inputDir.y += (inputs[0] ? 1 : 0) - (inputs[1] ? 1 : 0);
        _inputDir.x += (inputs[2] ? 1 : 0) - (inputs[3] ? 1 : 0);
        _inputDir.z += (inputs[4] ? 1 : 0);
        return _inputDir;
    }

    public void Move(Vector3 _inputDir)
    {
        forward = objectColider.ColisionNormal.forward;
        Vector3 _right = -objectColider.ColisionNormal.right;

        Vector3 _moveDir = _right * _inputDir.x + forward * _inputDir.y;
        if (_moveDir != Vector3.zero) _moveDir = Vector3.Normalize(_moveDir);

        moveVector = _moveDir; //Changed
        
        //Should now be moved by Velocity
        /*if (isGrounded && !Sliding)
            cc.Move(moveVector * moveSpeed * UnityEngine.Time.deltaTime);//Changed*/
            
        if (JumpVal != _inputDir.z) {
            holdJump = !holdJump;
        }
        
        JumpVal = _inputDir.z;
    }

    public void MoveDirect(Vector3 _Vector)
    {
        cc.Move(_Vector); //Changed
    }

    public void Teleport(Vector3 _position)
    {
        cc.enabled = false;
        transform.position = _position;
        cc.enabled = true;
    }

    public bool PreformJump(ref Vector3 veclocity)
    {
        if (isGrounded) //Jump Math
        {
            veclocity.y = 0;
            
            if (canJump)
            {
                //veclocity += moveVector * JumpVal * moveSpeed;
                veclocity.y += JumpVal * jumpHeight;
                isGrounded = false;
            }
        }
        else
        {
            canJump = false;
            if (veclocity.y >= 0 && holdJump)
            {
                return true;
            }
        }
        return false;
    }
}

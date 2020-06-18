using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : SmoothMovement
{
    [Header("User Information")]

    public string username;

    [HideInInspector] public int id;

    public int score = 0;

    public int deaths = 0;

    [Header("Player Stats")]

    public float health = 0;

    public int maxHealth = -1;

    [Header("Components")]

    public Text usernameText;

    public Slider Hpbar;

    public Transform playerModel;

    [Header("Animation Info")]

    public Animator animator;

    [HideInInspector] public int animationState = 0;

    [HideInInspector] public Vector2 subState = Vector2.zero;

    public void UpdateUserText()
    {
        if (usernameText != null)
            usernameText.text = username;

        InsertUpdate(PlayerUpdates);
    }

    public void SetInitStats(int _max, int _hp)
    {
        maxHealth = _max;
        health = _hp;
        if (Hpbar != null)
        {
            Hpbar.maxValue = _max;
        }
    }

    public void SetLocalHP()
    {
        Hpbar = UIManager.instance.localHpbar;
    }

    private void PlayerUpdates()
    {
        if (Hpbar != null)
        {
            Hpbar.value = health;
        }

        if (playerModel != null)
        {
            Vector3 lookvector = transform.forward;

            /*if (animationState == -1)
            {
                lookvector = target - transform.position;
                lookvector.y = 0;
                lookvector.Normalize();
            }*/

            playerModel.LookAt(playerModel.position + Vector3.Lerp(lookvector, playerModel.forward, .8f).normalized);
        }
            
        Animation();
    }

    private void Animation()
    {
        if (animator == null)
            return;

        if (animationState == 1)
        {
            animator.speed = 2;
        } else
        {
            animator.speed = 1;
        }

        //animator.SetLayerWeight(2, animationState == 4?0:1);
        //animator.SetLayerWeight(1, animationState == 4 ? 1 : 0);

        animator.SetInteger("animationState", animationState);
        animator.SetFloat("xSubState", subState.x);
        animator.SetFloat("ySubState", subState.y);
    }

    public bool isLowered()
    {
        return animationState == 4 || animationState == 2;
    }
}

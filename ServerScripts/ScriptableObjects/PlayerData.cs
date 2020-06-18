using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Name", menuName = "New Player Stats")]
public class PlayerData : ScriptableObject
{
    [Header("Player Movement")]
    public float baseMu = .5f;

    public float crouchMu = .1f;

    public float baseMoveSpeed = 5f;

    public float runModifer = 2;

    [Header("Player Stats")]
    public float Damage = 10f;
}

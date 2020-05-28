using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Drawing;

namespace GameServer
{
    class ObjectData : MonoBehaviour
    {
        [HideInInspector] public int id = -1;

        [HideInInspector] public bool death = false;

        [HideInInspector] public int owner = -1;

        [HideInInspector] public int createdTick = 1;

        [HideInInspector] public Vector3 moveVector = Vector3.zero;

        [HideInInspector] public float moveSpeed;

    } //End class
}

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    public class Time : MonoBehaviour
    {
        public static Time instance;

        public Time()
        {
            if (instance == null)
                instance = this;
        }

        public int tick = 0;

        public float time = 0;

        public void Update()
        {
            tick += 1;
            time = (float)tick / Constants.TICKS_PER_SEC;
        }
    }
}

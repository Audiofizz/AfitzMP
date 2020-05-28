using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    class GameLogic : MonoBehaviour
    {
        public static Dictionary<int, Plane> Planes = new Dictionary<int, Plane>();

        public static int PlaneCount = 2;

        public static void CreatePlanes()
        {
            Plane SumoPlane = new Plane(25, "Ground");
            Planes.Add(0, SumoPlane);

            Plane DeathPlane = new Plane(200, "Death", new Vector3(0,-25,0));
            Planes.Add(1, DeathPlane);
        }

        private void Start()
        {
            //CreatePlanes();
            Server.Start(50, 26950);
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }
    }
}

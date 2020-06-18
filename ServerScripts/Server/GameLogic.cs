using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    class GameLogic : MonoBehaviour
    {
        private void Start()
        {
            Server.Start(50, 26950);
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }
    }
}

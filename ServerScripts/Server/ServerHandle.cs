using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        { 
            int clientIDCheck = packet.ReadInt();
            string username = packet.ReadString();

            if (Server.clients[fromClient] == null)
            {
                Debug.Log($"Client Does not exist");
            }
            Debug.Log($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
            if (fromClient != clientIDCheck)
            {
                Debug.Log($"Player \"{ username }\" (ID: {fromClient}) has assumed the wrong client ID ({clientIDCheck})!");
            }
            Server.clients[fromClient].SendIntoGame(username);
        }

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            Server.clients[fromClient].player.inputTick = (int)(packet.ReadFloat() * Constants.TICKS_PER_SEC);

            bool[] _inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = packet.ReadBool();
            }

            if (Server.clients[fromClient].player != null)
                Server.clients[fromClient].player.SetInput(_inputs);
        }

        public static void PlayerRotation(int fromClient, Packet _packet)
        {
            Quaternion _noYawRotation = _packet.ReadQuaternion();
            Quaternion _rotation = _packet.ReadQuaternion();

            if (Server.clients[fromClient].player != null)
            {
                Server.clients[fromClient].player.Head.rotation = _rotation;
                Server.clients[fromClient].player.SetRotation(_noYawRotation);
            }
        }

        //NOW UDP
        public static void RequestPing(int fromClient, Packet packet) 
        {
            int id = packet.ReadInt();
            float time = packet.ReadFloat();
            ServerSend.SendPing(id, time);
        }

        public static void PlayerDisconnect(int fromClient, Packet packet)
        {
            /*int id = packet.ReadInt();
            Client temp;

            if (Server.clients.TryGetValue(id, out temp))
                temp.Disconnect();*/
        }
    }
}

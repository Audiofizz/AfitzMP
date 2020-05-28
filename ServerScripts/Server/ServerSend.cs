using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    class ServerSend
    {
        #region Decapricated UDP
        private static void SendUDPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].udp.SendData(packet);
        }

        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(packet);
            }
        }

        private static void SendUDPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                    Server.clients[i].udp.SendData(packet);
            }
        }

        #endregion
        private static void SendTCPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                Server.clients[i].tcp.SendData(packet);
            }
        }

        public static void Welcome(int toClient, string msg)
        {
            using(Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(msg);
                _packet.Write(toClient);

                SendTCPData(toClient, _packet);
            }
        }

        public static void SpawnPlayer(int toClient, Player player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(player.id);
                _packet.Write(player.username);
                _packet.Write(player.transform.position);
                _packet.Write(player.transform.rotation);

                SendTCPData(toClient, _packet);
            }

            InitPlayerStats(toClient, player);
        }

        public static void PlayerPosition(Player player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
            {
                _packet.Write(player.id);
                _packet.Write(player.transform.position);

                SendTCPDataToAll(_packet);
            }
        }

        public static void PlayerRotation(Player player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerRoation))
            {
                _packet.Write(player.id);
                _packet.Write(player.transform.rotation);

                SendTCPDataToAll(player.id, _packet);
            }
        }

        public static void DisconnectPlayer(int disconnectID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.disconnect))
            {
                _packet.Write(disconnectID);
                
                SendTCPDataToAll(_packet);
            }
        }

        public static void SendPing(int toClient, float time)
        {
            using (Packet _packet = new Packet((int)ServerPackets.receivePing))
            {
                _packet.Write(time);
                _packet.Write(UnityEngine.Time.time);

                SendTCPData(toClient, _packet);
            }
        }

        public static void SpawnItem(int toClient,Projectile item)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnItem))
            {
                _packet.Write(item.id);
                _packet.Write(item.transform.position);
                _packet.Write(item.veclocity);

                SendTCPData(toClient,_packet);
            }
        }

        public static void DestroyProjectile(int ItemID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.destroyItem))
            {
                _packet.Write(ItemID);

                SendTCPDataToAll(_packet);
            }
        }

        public static void SpawnItem(Projectile item)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnItem))
            {
                _packet.Write(item.id);
                _packet.Write(item.transform.position);
                _packet.Write(item.veclocity);

                SendTCPDataToAll(_packet);
            }
        }

        public static void playerAnimation(Player player, float xsub, float ysub)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerAnimation))
            {
                _packet.Write(player.id);
                _packet.Write(player.animationState);
                _packet.Write(xsub);
                _packet.Write(ysub);

                SendTCPDataToAll(_packet);
            }
        }

        public static void UpdateHealth(Player player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.updateHealth))
            {
                _packet.Write(player.id);
                _packet.Write(player.health);

                SendTCPDataToAll(_packet);
            }
        }

        public static void HitDamageObject(int toClient, bool Killed)
        {
            using (Packet _packet = new Packet((int)ServerPackets.hitDamageObject))
            {
                _packet.Write(Killed);

                SendTCPData(toClient, _packet);
            }
        }

        public static void UpdateScore(Player player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.updateScore))
            {
                _packet.Write(player.id);
                _packet.Write(player.score);
                _packet.Write(player.deaths);

                SendTCPDataToAll(_packet);
            }
        }

        public static void InitPlayerStats(int toClient, Player player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.initPlayerStats))
            {
                _packet.Write(player.id);
                _packet.Write(player.maxHealth);
                _packet.Write(player.health);

                SendTCPData(toClient,_packet);
            }
        }

        public static void SpawnObject(int toClient, Transform transform, int UID, int ID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnObject))
            {
                _packet.Write(ID);
                _packet.Write(UID);
                _packet.Write(transform.position);
                _packet.Write(transform.rotation);

                SendTCPData(toClient, _packet);
            }
        }

        public static void SpawnObject(Transform transform, int UID, int ID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnObject))
            {
                _packet.Write(ID);
                _packet.Write(UID);
                _packet.Write(transform.position);
                _packet.Write(transform.rotation);

                SendTCPDataToAll(_packet);
            }
        }

        public static void DestroyObject(int ID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.destroyObject))
            {
                _packet.Write(ID);

                SendTCPDataToAll(_packet);
            }
        }
        public static void ProjectileHit(Vector3 startpos, Vector3 hitpos, int upid)
        {
            using (Packet _packet = new Packet((int)ServerPackets.projectileHit))
            {
                _packet.Write(upid);
                _packet.Write(startpos);
                _packet.Write(hitpos);

                SendTCPDataToAll(_packet);
            }
        }
    }
}

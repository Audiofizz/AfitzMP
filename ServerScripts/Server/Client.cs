using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace GameServer
{
    class Client
    {
        public int id;
        public TCP tcp;
        public UDP udp;

        public Player player;

        public static int dataBufferSize = 4096;

        public Client(int id)
        {
            this.id = id;
            this.tcp = new TCP(this.id);
            this.udp = new UDP(this.id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;

            private Packet recievedData;

            private byte[] receiveBuffer;

            public TCP(int id)
            {
                this.id = id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                recievedData = new Packet();

                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                recievedData = null;
                receiveBuffer = null;
                socket = null;
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error sending data to player {id} via TCP: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult asyncResult)
            {
                try
                {
                    int byteLength = stream.EndRead(asyncResult);
                    if (byteLength <= 0)
                    {
                        if (Server.clients[id].player != null)
                            Server.clients[id].Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    recievedData.Reset(HandleData(data));

                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error receiving TCP data {ex}");
                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetLength = 0;

                recievedData.SetBytes(data);

                if (recievedData.UnreadLength() >= 4)
                {
                    packetLength = recievedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (packetLength > 0 && packetLength <= recievedData.UnreadLength())
                {
                    byte[] packetBytes = recievedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetID = packet.ReadInt();
                            Server.packetHandlers[packetID](id, packet);
                        }
                    });

                    packetLength = 0;

                    if (recievedData.UnreadLength() >= 4)
                    {
                        packetLength = recievedData.ReadInt();
                        if (packetLength <= 0)
                        {
                            return true;
                        }
                    }

                }

                if (packetLength <= 1)
                {
                    return true;
                }

                return false;

            }

        }

        #region Decapricated UDP
        public class UDP
        {
            public IPEndPoint endPoint;

            public int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Disconnect()
            {
                endPoint = null;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(Packet packet)
            {
                Server.SendUDPData(endPoint, packet);
            }

            public void HandleData(Packet packet)
            {
                int _packetLength = packet.ReadInt();
                byte[] _packetBytes = packet.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int packetID = _packet.ReadInt();
                        Server.packetHandlers[packetID](id, _packet);
                    }
                });
            }
        }
        #endregion

        public void SendIntoGame(string _playerName)
        {
            player = Prefabs.instance.InstantiatePrefab<Player>(Prefabs.instance.Player);
            if (id % 2 == 0)
                player.teamdex = 2;
            player.Initialize(id, _playerName, new Vector3(0, 0, 0));
            

            foreach (Client _client in Server.clients.Values) //Send All Players to Connector including the Connector's Player
            {
                if (_client.player != null)
                {
                    ServerSend.SpawnPlayer(id, _client.player);
                }
            }

            foreach (Client _client in Server.clients.Values) //Send Connector's Players to all Clients
            {
                if (_client.player != null)
                {
                    if (_client.id != id) //Make sure we do not sent this to Connector
                    {
                        ServerSend.SpawnPlayer(_client.id, player);
                    }
                }
            }

            LoadQueue.PreformLoadQueue(id);
        }

        public void Disconnect()
        {
            Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected");

            ServerSend.DisconnectPlayer(id);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                UnityEngine.Object.Destroy(player.gameObject);
                player = null;
            });

            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}

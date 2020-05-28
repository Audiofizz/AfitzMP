using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace GameServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }

        public static int MaxItems = 200;

        public static int MaxObjects = 500;
        public static int Port { get; private set; }

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static TcpListener tcpListener;
        public static UdpClient udpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Debug.Log("Starting server...");
            InitServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            #region Decapricated UDP
            //udpListener = new UdpClient(Port);
            //udpListener.BeginReceive(UDPReceiveCallback, null);
            #endregion

            Debug.Log($"Server started on {Port}.");
        }

        public static void DisconnectClients()
        {
            foreach (Client client in Server.clients.Values)
            {
                ServerSend.DisconnectPlayer(client.id);
            }
            Server.clients.Clear();
        }

        private static void TCPConnectCallback(IAsyncResult asyncResult)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(asyncResult);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);
                    return;
                }
            }

            Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full");
        }

        #region Decapricated UDP

        private static void UDPReceiveCallback(IAsyncResult asyncResult)
        {
            IPEndPoint _clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                /*_clientEndpoint = new IPEndPoint(IPAddress.Any, 0);*/
                byte[] data = udpListener.EndReceive(asyncResult, ref _clientEndpoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (Packet packet = new Packet(data))
                {
                    int _clientID = packet.ReadInt();

                    if (_clientID == 0)
                    {
                        return;
                    }

                    if (clients[_clientID].udp.endPoint == null)
                    {
                        Debug.Log($"New UDP Connection to {_clientEndpoint}");
                        clients[_clientID].udp.Connect(_clientEndpoint);
                        return;
                    }

                    if (clients[_clientID].udp.endPoint.ToString() == _clientEndpoint.ToString())
                    {
                        clients[_clientID].udp.HandleData(packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiveing UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet packet)
        {
            try
            {
                //Debug.Log($"Sending UDP data to {_clientEndPoint}");
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Sending UDP data to {_clientEndPoint} via UDP: {ex}");
            }
        }

        #endregion

        private static void InitServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }
/*            for (int i = 1; i <= MaxItems; i++)
            {
                items.Add(i, new Item(i));
            }*/

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived,ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement,ServerHandle.PlayerMovement },
                { (int)ClientPackets.requestPing,ServerHandle.RequestPing },
                { (int)ClientPackets.playerRotation,ServerHandle.PlayerRotation },
            };
            Debug.Log("Initialized packets.");
        }

        public static void Stop()
        {
            tcpListener.Stop();
            //udpListener.Close();
        }
    }
}

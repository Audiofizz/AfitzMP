using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists");
            Destroy(this);
        }
    }

    public static int dataBufferSize = 4069;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;

    public float ping;

    public float lastServerTime;
    public float lastServerTimeLocal;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public bool Connected()
    {
        return isConnected;
    }

    public void ConnectToServer()
    {
        UIUpdateIP();
        InitClientData();
        tcp.Connect();
    }

    public static void DisconnectFromServer()
    {
        instance.Disconnect();
        UIManager.instance.DisconnectedUI();
    }

    private void UIUpdateIP()
    {
        if (UIManager.instance.ipAddressField != null)
        {
            if (UIManager.instance.ipAddressField.text != null || UIManager.instance.ipAddressField.text != "")
            {
                ip = UIManager.instance.ipAddressField.text;
            }
        }
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet recivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];

            Debug.Log($"Connecting via {instance.ip}:{instance.port}");
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                socket.EndConnect(asyncResult);
            } catch (Exception e) {
                Debug.Log($"Error on connect with {e}");
            }

            if(!socket.Connected)
            {
                Debug.Log("Enabling menu!");
                UIManager.instance.MainMenu(true);
                Debug.Log("Could not Connect!");
                return;
            } else
            {
                instance.isConnected = true;
                Debug.Log("Connected!");
            } 

            stream = socket.GetStream();

            recivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

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
                Debug.Log($"Error sending data to server via TCP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                int byteLength = stream.EndRead(asyncResult);
                if (byteLength <= 0)
                {
                    DisconnectFromServer();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                recivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                DisconnectFromServer();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0; 

            recivedData.SetBytes(data);

            if (recivedData.UnreadLength() >= 4)
            {
                packetLength = recivedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= recivedData.UnreadLength())
            {
                byte[] packetBytes = recivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        packetHandlers[packetID](packet);
                    }
                });

                packetLength = 0;

                if (recivedData.UnreadLength() >= 4)
                {
                    packetLength = recivedData.ReadInt();
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

        public void Disconnect()
        {
            stream = null;
            recivedData = null;
            receiveBuffer = null;
            socket = null;
        }

    }

    #region Decapricated UDP
    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet())
            {
                SendData(packet);
            }
        }

        public void Disconnect()
        {
            socket = null;
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                byte[] data = socket.EndReceive(asyncResult, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    DisconnectFromServer();
                    return;
                }
                HandleData(data);
            }
            catch
            {
                DisconnectFromServer();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet packet = new Packet(_data))
            {
                int _packetLength = packet.ReadInt();
                _data = packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(_data))
                {
                    int packetID = packet.ReadInt();
                    packetHandlers[packetID](packet);
                }
            });
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }

            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to server via UDP: {ex}");
            }
        }

        
    }
    #endregion

    private void InitClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRoation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.disconnect, ClientHandle.DisconnectPlayer },
            { (int)ServerPackets.receivePing, ClientHandle.ReceivePing },
            { (int)ServerPackets.spawnItem, ClientHandle.SpawnItem },
            { (int)ServerPackets.destroyItem, ClientHandle.DestroyItem },
            { (int)ServerPackets.playerAnimation, ClientHandle.PlayerAnimation },
            { (int)ServerPackets.updateHealth, ClientHandle.UpdateHealth },
            { (int)ServerPackets.initPlayerStats, ClientHandle.InitPlayerStats },
            { (int)ServerPackets.updateScore, ClientHandle.UpdateScore },
            { (int)ServerPackets.hitDamageObject, ClientHandle.HitDamageObject },
            { (int)ServerPackets.spawnObject, ClientHandle.SpawnObject },
            { (int)ServerPackets.destroyObject, ClientHandle.DestroyObject },
            { (int)ServerPackets.projectileHit, ClientHandle.ProjectileHit }
        };

        Debug.Log("Initialized packets.");

    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            //udp.socket.Close();
            tcp.Disconnect();
            //udp.Disconnect();

            Debug.Log("Disconnected from server.");
        }
    }
}

using System;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    #region Decapricated UDP
    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }
    #endregion

    #region Packets

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(Client.instance.lastServerTime + (Time.time - Client.instance.lastServerTimeLocal));

            packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                packet.Write(_input);
            }
            
            SendTCPData(packet);
        }
    }

    public static void PlayerRotation()
    {
        using (Packet packet = new Packet((int)ClientPackets.playerRotation))
        {
            packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
            packet.Write(GameManager.localCamera.transform.rotation);
            SendTCPData(packet);
        }
    }

    public static void RequestPing()
    {
        if (Client.instance.myId == 0)
            return;

        using (Packet packet = new Packet((int)ClientPackets.requestPing))
        {
            packet.Write(Client.instance.myId);
            packet.Write(Time.time);

            SendTCPData(packet);
        }
    }

    public static void PlayerDisconnect()
    {
        using (Packet packet = new Packet((int)ClientPackets.playerDisconnect))
        {
            packet.Write(Client.instance.myId);

            SendTCPData(packet);
        }
    }

    #endregion
}

using System;
using UnityEngine;
using System.Net;

public class ClientHandle : MonoBehaviour
{
     
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myID = packet.ReadInt();

        Debug.Log($"Message from server: {msg}");
        Client.instance.myId = myID;

        ClientSend.WelcomeReceived();
        UIManager.instance.DisableAllMenuUI();

        //Decapricated UDP
        //Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void DisconnectPlayer(Packet packet)
    {
        int idDisconnect = packet.ReadInt();

        GameManager.instance.DisconnectPlayer(idDisconnect);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int _id = packet.ReadInt();
        string _username = packet.ReadString();
        Vector3 _pos = packet.ReadVector3();
        Quaternion _rot = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _pos, _rot);
    }

    public static void PlayerPosition(Packet packet)
    {
        int _id = packet.ReadInt();
        Vector3 _pos = packet.ReadVector3();

        if (GameManager.players.ContainsKey(_id))
        {
            if (!GameManager.players[_id].gameObject.activeSelf)
                GameManager.players[_id].gameObject.SetActive(true);

            GameManager.players[_id].SetTargetPosition(_pos);
        }
            
    }

    public static void SpawnItem(Packet packet)
    {
        int _id = packet.ReadInt();
        Vector3 _pos = packet.ReadVector3();
        Vector3 _velocity = packet.ReadVector3();

        if (!GameManager.items.ContainsKey(_id))
        {
            GameManager.instance.CreateProjectile(_pos, _velocity, _id);
        } 
    }

    public static void DestroyItem(Packet packet)
    {
        int _id = packet.ReadInt();

        if (!Projectile.projectiles.ContainsKey(_id))
            return;

        if (Projectile.projectiles[_id].deathPrfab != null)
        {
            GameObject temp = Instantiate(Projectile.projectiles[_id].deathPrfab);
            temp.transform.position = Projectile.projectiles[_id].transform.position;
            Destroy(temp, 1);
        }

        Destroy(Projectile.projectiles[_id].gameObject);
    }

    public static void PlayerRotation(Packet packet)
    {
        int _id = packet.ReadInt();
        Quaternion _rot = packet.ReadQuaternion();

        if (GameManager.localPlayer != null)
            if (GameManager.localPlayer.id == _id)
                return;

        if (GameManager.players.ContainsKey(_id))
            GameManager.players[_id].transform.rotation = _rot;
    }

    public static void ReceivePing(Packet packet)
    {
        float TimeOriginalySent = packet.ReadFloat();
        float ServerTime = packet.ReadFloat();

        Client.instance.ping = Time.time - TimeOriginalySent;
        Client.instance.lastServerTime = ServerTime;
        Client.instance.lastServerTimeLocal = Time.time;
        PingManager.instance.UpdatePingText(Client.instance.ping);
    }

    public static void PlayerAnimation(Packet packet)
    {
        int _id = packet.ReadInt();
        int _state = packet.ReadInt();
        Vector2 _stateSub = new Vector2(packet.ReadFloat(), packet.ReadFloat());

        if (GameManager.players.ContainsKey(_id))
        {
            GameManager.players[_id].animationState = _state;
            GameManager.players[_id].subState = Vector2.Lerp(_stateSub, GameManager.players[_id].subState, .7f);
        }
    }

    public static void UpdateHealth(Packet packet)
    {
        int _id = packet.ReadInt();
        float _health = packet.ReadFloat();

        if (!GameManager.players.ContainsKey(_id))
            return;

        if (GameManager.players[_id].health > _health && _id == GameManager.localPlayer.id)
            UIManager.instance.TakeDamage();

        GameManager.players[_id].health = _health;
    }

    public static void InitPlayerStats(Packet packet)
    {
        int _id = packet.ReadInt();
        int _maxhealth = packet.ReadInt();
        int _health = packet.ReadInt();

        if (GameManager.players.ContainsKey(_id))
        {
            GameManager.players[_id].SetInitStats(_maxhealth, _health);
        }
    }

    public static void UpdateScore(Packet packet)
    {
        int _id = packet.ReadInt();
        int _score = packet.ReadInt();
        int _deaths = packet.ReadInt();

        if (GameManager.players.ContainsKey(_id))
        {
            GameManager.players[_id].score = _score;
            GameManager.players[_id].deaths = _deaths;
            Leaderboard.instance.UpdateScore();
        }
    }

    public static void HitDamageObject(Packet packet)
    {
        bool _Kill = packet.ReadBool();

        UIManager.instance.HitMarker();
        if (_Kill)
            UIManager.instance.GotKill();
    }

    public static void SpawnObject(Packet packet)
    {
        int _id = packet.ReadInt();
        int _uid = packet.ReadInt();
        Vector3 _pos = packet.ReadVector3();
        Quaternion _rot = packet.ReadQuaternion();

        if (_uid > Prefabs.instance.Objects.Length - 1)
            return;

        GameObject temp = Instantiate(Prefabs.instance.Objects[_uid], _pos, _rot);

        GameManager.Objects.Add(_id, temp);
    }

    public static void DestroyObject(Packet packet)
    {
        int _id = packet.ReadInt();

        if (!GameManager.Objects.ContainsKey(_id))
            return;

        ExplodeOnDeath temp = GameManager.Objects[_id].GetComponent<ExplodeOnDeath>();
        if (temp != null)
        {
            temp.Explode();
        }

        Destroy(GameManager.Objects[_id]);
        GameManager.Objects.Remove(_id);
    }

    public static void ProjectileHit(Packet packet)
    {
        int _upid = packet.ReadInt();
        Vector3 _Startpoint = packet.ReadVector3();
        Vector3 _Hitpoint = packet.ReadVector3();

        Projectiles.SpawnProjectile(_upid,_Startpoint,_Hitpoint);
    }


}

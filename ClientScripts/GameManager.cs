using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public static Dictionary<int, Projectile> items = new Dictionary<int, Projectile>();

    public static Dictionary<int, GameObject> Objects = new Dictionary<int, GameObject>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject RockPrefab;

    public static PlayerManager localPlayer;

    public static Camera localCamera;

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

    public void CreateProjectile(Vector3 _pos,Vector3 _velocity, int id)
    {
        Projectile projectile = Instantiate(RockPrefab).GetComponent<Projectile>();
        projectile.SetValues(_pos, _velocity, id);
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _pos, Quaternion _rot)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            //This is the local player
            _player = Instantiate(localPlayerPrefab, _pos, _rot);
            localPlayer = _player.GetComponent<PlayerManager>();
            localPlayer.SetLocalHP();
            localCamera = _player.GetComponentInChildren<Camera>();
        } else
        {
            //This is a non local player
            _player = Instantiate(playerPrefab, _pos, _rot);
        }

        PlayerManager _playerMan = _player.GetComponent<PlayerManager>();
        _playerMan.id = _id;
        _playerMan.username = _username;
        _playerMan.UpdateUserText();
        players.Add(_id, _playerMan);
    }

    public void DisconnectPlayer(int idDisconnected)
    {
        Destroy(players[idDisconnected].gameObject);
        GameManager.players.Remove(idDisconnected);
    }
}

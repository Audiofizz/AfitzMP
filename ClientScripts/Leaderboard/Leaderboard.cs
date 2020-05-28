using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard instance;

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

    Dictionary<int,LeaderboardPlayer> leaderboard;

    public GameObject playerleaderboardPrefab;

    public Transform panel;

    public GameObject render;

    public void UpdateScore()
    {
        if (leaderboard == null)
            leaderboard = new Dictionary<int, LeaderboardPlayer>();

        foreach (PlayerManager player in GameManager.players.Values)
        {
            if (!leaderboard.ContainsKey(player.id))
            {
                LeaderboardPlayer temp = Instantiate(playerleaderboardPrefab, panel).GetComponent<LeaderboardPlayer>();
                
                leaderboard.Add(player.id, temp);
            }

            if (leaderboard.ContainsKey(player.id))
                leaderboard[player.id].UpdateScore(player);

        }
    }

    public void Active(bool active)
    {
        render.SetActive(active);
    }
}

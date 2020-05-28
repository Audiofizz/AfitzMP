using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPlayer : MonoBehaviour
{
    public Text username;
    public Text score;
    public Text deaths;

    public void UpdateScore(PlayerManager player)
    {
        username.text = player.username;
        score.text = "" + player.score;
        deaths.text = "" + player.deaths;
    }
}

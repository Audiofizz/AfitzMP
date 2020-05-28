using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PingManager : MonoBehaviour
{
    public static PingManager instance;
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

    public Text Ping;

    private float timeOfNextPing;
    public int timeBetweenPingsMS;
    private float timeOfNextTextChange;
    public int timeBetweenPingsDisplayed;

    private List<float> pastPing;

    public int pingListSize = 10;

    private void Start()
    {
        timeOfNextPing = Time.time;
        timeOfNextTextChange = timeOfNextPing;
        pastPing = new List<float>();
        pastPing.Insert(0, 0);
    }

    public void Update()
    {
        //TestPing();
    }

    private void TestPing()
    {
        if (Client.instance.Connected() && timeOfNextPing <= Time.time)
        {
            timeOfNextPing = Time.time + (timeBetweenPingsMS/1000f);
            ClientSend.RequestPing();
        }
    }

    public void UpdatePingText(float value)
    {
        int count = pastPing.Count;
        if (pastPing.Count == pingListSize)
        {
            pastPing.RemoveAt(0);
        } else if (pastPing.Count > pingListSize)
            pastPing.Clear();

        pastPing.Insert(0, value);

        UpdateTextValue((int)(pastPing.Average() * 1000));
    }
    private void UpdateTextValue(int value)
    {
        if (timeOfNextTextChange <= Time.time)
        {
            Ping.text = $"Ping:[{value}ms]";
            timeOfNextTextChange = Time.time + timeBetweenPingsDisplayed;
        }
        
    }
}

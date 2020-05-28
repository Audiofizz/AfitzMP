using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class Prefabs : MonoBehaviour
{
    public static Prefabs instance;
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

    public GameObject[] Objects;
    public GameObject[] Effects;
    public GameObject[] DeathParticals;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameServer;

public class Barrel : MonoBehaviour, Damageable, ExplodeOnDeath
{
    private MeshRenderer meshRenderer;
    private new Collider collider;

    [SerializeField] private float respawnTime = 5f;

    private float respawnAtTime = 0;

    private bool destroyed = false;

    static int idCounter = 0;
    private int id = -1; //Object Identifier
    private int UID = 1; //Object Type Identifiy

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    private void Awake()
    {
        id = idCounter;
        idCounter++;
        LoadQueue.AddLoadToQueue(SpawnObject);
    }

    private void SpawnObject(int toclient)
    {
        if (!destroyed)
            ServerSend.SpawnObject(toclient,transform, UID, id);
    }

    public bool TakeDamage(float amount, int owner)
    {
        if (owner >= 0)
            ServerSend.HitDamageObject(owner, true);
        SafeDestory();
        return true;
    }

    public void Explode()
    {
        GameObject temp = Instantiate(Prefabs.instance.Effects[0]);
        temp.transform.position = transform.position;
    }

    private void SafeDestory()
    {
        Active(false);
        respawnAtTime = UnityEngine.Time.time + respawnTime;
        Explode();

        ServerSend.DestroyObject(id);

        //LoadQueue.RemoveLoadFromQueue(SpawnObject);
    }

    private void Respawn()
    {
        ServerSend.SpawnObject(transform, UID, id);
        Active(true);
    }

    private void Active(bool state)
    {
        collider.enabled = state;
        meshRenderer.enabled = state;
        destroyed = !state;
    }

    private void Update()
    {
        if (UnityEngine.Time.time >= respawnAtTime && destroyed)
        {
            Respawn();
        }
    }

}

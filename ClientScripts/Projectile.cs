using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int,Projectile> projectiles = new Dictionary<int, Projectile>();

    public int id;

    public float speed = 1;

    public float Lifetime = 60;

    float DeathTime;

    public GameObject deathPrfab;

    Vector3 velocity = Vector3.zero;

    public Rigidbody rigidbody;

    private void Start()
    {
        transform.LookAt(transform.position);
    }

    private void Awake()
    {
        DeathTime = Time.time + Lifetime;
    }

    public void SetValues(Vector3 _pos, Vector3 _velocity, int _id)
    {
        transform.position = _pos;
        transform.LookAt(transform.position + _velocity);
        id = _id;
        projectiles.Add(id, this);
        rigidbody.velocity = _velocity*speed;
    }

    private void OnDestroy()
    {
        projectiles.Remove(id);
    }

    // Update is called once per frame
    /*    void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;

            if (DeathTime <= Time.time)
            {
                DestroyImmediate(this.gameObject);
            }
        }*/
}

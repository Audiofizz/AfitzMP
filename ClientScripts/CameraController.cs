using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

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

    public bool focused = true;

    public float headHeight = 1.5f;

    public PlayerManager player;
    public float sensitivity = 100f;
    public float clampAngle = 85f;

    public float vertRotation;
    public float horzRotation;

    public void Start()
    {
        vertRotation = transform.localEulerAngles.x;
        horzRotation = player.transform.eulerAngles.y;
    }

    private void Update()
    {
        Look();
        Vector3 temp = transform.position;
        temp.y = player.transform.position.y + (player.isLowered() ? headHeight / 2 : headHeight);
        transform.position = temp;
        //Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
    }

    private void Look()
    {
        if (!focused)
            return;
        float _mouseVert = -Input.GetAxis("Mouse Y");
        float _mouseHorz = Input.GetAxis("Mouse X");

        vertRotation += _mouseVert * sensitivity * Time.deltaTime;
        horzRotation += _mouseHorz * sensitivity * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(vertRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f,horzRotation,0f);
    }

}

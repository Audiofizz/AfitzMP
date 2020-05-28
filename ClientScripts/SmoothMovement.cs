using System;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    private Vector3 target = Vector3.zero;
    public static float lerpValue = .5f;

    private float viewClamp = 0.525321989f;

    private Action updateCallback;

    private Vector3 velocity;

    public void SetInitalValues(Vector3 _target, Vector3 _velocity)
    {
        velocity = _velocity;
        transform.position = _target;
        target = _target;
    }

    public void SetTargetPosition(Vector3 positon)
    {
        target = positon;
    }

    private void LerpTargetPosition()
    {
        transform.position = Vector3.Lerp(transform.position, target, lerpValue);
    }

    private void Update()
    {
        LerpTargetPosition();

        if (GameManager.localPlayer == null)
            goto Update;

        if (transform == GameManager.localPlayer.transform)
            goto Update;

        if (Vector3.Dot(GameManager.localPlayer.transform.forward, (transform.position - GameManager.localPlayer.transform.position).normalized) < viewClamp)
        {
            gameObject.SetActive(false);
        }

        Update:
        updateCallback?.Invoke();
    }

    public void InsertUpdate(Action callback)
    {
        updateCallback = callback;
    }
}

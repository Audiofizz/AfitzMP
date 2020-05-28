using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneObject : MonoBehaviour
{

    [SerializeField] private Renderer pRenderer;

    public void Initialize(Vector3 pos, float width, float length)
    {
        transform.position = pos + new Vector3(width / 2f, 0, length / 2f);
        transform.localScale = new Vector3(width / 10f, 1, length / 10f);
        pRenderer.material.mainTextureScale = new Vector2(width / 10f, length / 10f);
    }
}

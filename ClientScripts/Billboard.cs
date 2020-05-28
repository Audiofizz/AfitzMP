using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.localCamera != null)
        {
            transform.LookAt(GameManager.localCamera.transform.position);
        }
            
    }
}

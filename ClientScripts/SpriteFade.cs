using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFade : MonoBehaviour
{
    private void Awake()
    {
        image = GetComponent<Image>();
        Activate();
    }

    private bool active = false;

    private float alpha = 0;

    private Image image;

    private Color color; 

    void Update()
    {
        if (active)
        {
            alpha -= .02f;
            if (alpha <= 0)
            {
                alpha = 0;
                active = false;
            }

            color.a = alpha;
            image.color = color;
        }
    }

    public void Activate()
    {
        color = image.color;
        alpha = 1;
        active = true;
    }
}

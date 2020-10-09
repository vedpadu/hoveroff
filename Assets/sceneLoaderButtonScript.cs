using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneLoaderButtonScript : MonoBehaviour
{
    public float scaleHovered;

    public float lerpValueButton;
    private float currentScale;

    private bool mouseEntered;

    private float initScale;
    // Start is called before the first frame update
    void Start()
    {
        currentScale = transform.localScale.x;
        initScale = currentScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
        if (mouseEntered)
        {
            currentScale = Mathf.Lerp(currentScale, scaleHovered, lerpValueButton);
        }
        else
        {
            currentScale = Mathf.Lerp(currentScale, initScale, lerpValueButton);
        }
    }

    private void OnMouseEnter()
    {
        mouseEntered = true;
    }

    private void OnMouseExit()
    {
        mouseEntered = false;
    }
}

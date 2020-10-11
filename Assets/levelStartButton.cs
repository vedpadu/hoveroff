using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelStartButton : MonoBehaviour
{
    public float hoveredScale;

    private float initScale;

    public float scaleChangeLerp;

    private float currentScale;

    private bool mouseInside;
    // Start is called before the first frame update
    void Start()
    {
        currentScale = transform.localScale.x;
        initScale = currentScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(currentScale, currentScale,1f);
        if (mouseInside)
        {
            currentScale = Mathf.Lerp(currentScale, hoveredScale, scaleChangeLerp);
            print("e");
        }
        else
        {
            currentScale = Mathf.Lerp(currentScale, initScale, scaleChangeLerp);
        }
    }

    private void OnMouseEnter()
    {
        print("enter");
        mouseInside = true;
    }

    private void OnMouseExit()
    {
        print("exit");
        mouseInside = false;
    }
}

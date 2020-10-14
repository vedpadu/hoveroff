using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitGameButtonScript : MonoBehaviour
{
    public float hoveredScale;

    private float initScale;

    private float currentScale;

    private bool mouseInside;

    public float lerpVal;
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
        if (mouseInside)
        {
            currentScale = Mathf.Lerp(currentScale, hoveredScale, lerpVal);
            if (Input.GetMouseButtonDown(0))
            {
                #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                #else
                      Application.Quit();
                #endif
            }
        }
        else
        {
            currentScale = Mathf.Lerp(currentScale, initScale, lerpVal);
        }
    }

    private void OnMouseEnter()
    {
        mouseInside = true;
    }

    void OnMouseExit()
    {
        mouseInside = false;
    }
}

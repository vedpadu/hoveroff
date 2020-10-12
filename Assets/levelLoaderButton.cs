﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelLoaderButton : MonoBehaviour
{
    public float scaleHovered;

    public float lerpValueButton;
    private float currentScale;

    private bool mouseEntered;

    private float initScale;

    public int scene;

    public int sceneTransition;

    private levelManagerScript levelLoader;

    public bool enabled = true;
    // Start is called before the first frame update
    void Start()
    {
        currentScale = transform.localScale.x;
        initScale = currentScale;
        levelLoader = GameObject.FindGameObjectWithTag("levelSelect").GetComponent<levelManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            if (mouseEntered)
            {
                currentScale = Mathf.Lerp(currentScale, scaleHovered, lerpValueButton);
                if (Input.GetMouseButtonDown(0))
                {
                    levelLoader.LoadNextLevel(scene, sceneTransition);
                    PlayerPrefs.SetInt("startGameImmediate", 0);
                }
            }
            else
            {
                currentScale = Mathf.Lerp(currentScale, initScale, lerpValueButton);
            }
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
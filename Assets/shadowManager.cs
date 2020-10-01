using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowManager : MonoBehaviour
{
    public Vector2 direction;
    public Color shadowColor;

    private void Awake()
    {
        direction = direction.normalized;
    }
}
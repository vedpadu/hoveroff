using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelSelectButtonScript : MonoBehaviour
{
    private bool mouseInside;

    public float hoveredScale;

    public Color clickedColor;

    private float initScale;

    public float scaleChangeLerp;

    private SpriteRenderer sR;

    private float currentScale;

    public levelWheelScript lWS;

    public bool goingRight;

    private bool disabled = false;

    public Color disabledColor;
    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale.x;
        currentScale = initScale;
        sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (goingRight)
        {
            if (lWS.targetIndex >= lWS.maxIndex)
            {
                disabled = true;
            }
            else
            {
                disabled = false;
            }
        }
        else
        {
            if (lWS.targetIndex <= 0)
            {
                disabled = true;
            }
            else
            {
                disabled = false;
            }
        }
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
        if (mouseInside)
        {
            if (!disabled)
            {
                currentScale = Mathf.Lerp(currentScale, hoveredScale, scaleChangeLerp);
            }
            else
            {
                currentScale = Mathf.Lerp(currentScale, initScale, scaleChangeLerp);
            }
           
            if (!disabled)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (goingRight)
                    {
                        lWS.targetIndex += 1;
                    }
                    else
                    {
                        lWS.targetIndex -= 1;
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    sR.color = clickedColor;
                }
                else
                {
                    sR.color = Color.white;
                }
            }
            else
            {
                sR.color = disabledColor;
            }
            
        }
        else
        {
            if (!disabled)
            {
                sR.color = Color.white;
            }
            else
            {
                sR.color = disabledColor;
            }
           
            currentScale = Mathf.Lerp(currentScale, initScale, scaleChangeLerp);
        }
    }

    private void OnMouseEnter()
    {
        mouseInside = true;
    }

    private void OnMouseExit()
    {
        mouseInside = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optionsButtonScript : MonoBehaviour
{
    public CanvasGroup cGroup;

    public bool menuOpen;

    public float hoveredScale;

    private float initScale;

    public float lerpVal;

    private bool mouseInside;
    float currentScale;

    public float canvasShowTime;
    // Start is called before the first frame update
    void Start()
    {
        currentScale = transform.localScale.x;
        initScale = currentScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (menuOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(CanvasDisappear(canvasShowTime));
                menuOpen = false;
            }
        }
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
        if (mouseInside)
        {
            currentScale = Mathf.Lerp(currentScale, hoveredScale, lerpVal);
            if (Input.GetMouseButtonDown(0) && !menuOpen)
            {
                StartCoroutine(CanvasShow(canvasShowTime));
                menuOpen = true;
            }
        }
        else
        {
            currentScale = Mathf.Lerp(currentScale, initScale, lerpVal);
        }
    }

    IEnumerator CanvasShow(float duration)
    {
        float elapsed = 0.0f;
        cGroup.gameObject.SetActive(true);
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            cGroup.alpha = alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }

        cGroup.alpha = 1f;
    }

    IEnumerator CanvasDisappear(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            cGroup.alpha = alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }

        cGroup.alpha = 0f;
        cGroup.gameObject.SetActive(false);
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

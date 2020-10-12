using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenuScript : MonoBehaviour
{
    public AnimationCurve growCurve;

    public float growDuration;

    public bool optionsOpen;

    public bool paused;

    private cameraShake camerShake;

    void Start()
    {
        camerShake = Camera.main.GetComponent<cameraShake>();
    }
    // Start is called before the first frame update
    public void StartGrow()
    {
        camerShake = Camera.main.GetComponent<cameraShake>();
        camerShake.shakes.Clear();   
        StartCoroutine(Grow(growDuration));
        paused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!optionsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(Shrink(growDuration));
            }
        }
    }

    public IEnumerator Shrink(float duration)
    {
        float elapsed = 0.0f;
        float currentScale = 0.0f;
        while (elapsed < duration)
        {
            float lerpVal = growCurve.Evaluate(elapsed / duration);
            currentScale = Mathf.Lerp(0f, 1f, 1 - lerpVal);
            transform.localScale = new Vector3(currentScale, currentScale,1f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        transform.localScale = new Vector3(0f,0f,0f);
        paused = false;
        gameObject.SetActive(false);
    }
    
    IEnumerator Grow(float duration)
    {
        float elapsed = 0.0f;
        float currentScale = 0.0f;
        while (elapsed < duration)
        {
            float lerpVal = growCurve.Evaluate(elapsed / duration);
            currentScale = Mathf.Lerp(0f, 1f, lerpVal);
            transform.localScale = new Vector3(currentScale, currentScale,1f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1f,1f,1f);
    }
}

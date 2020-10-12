using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class cameraShake : MonoBehaviour
{
    [HideInInspector]public List<Shake> shakes = new List<Shake>();

    private void Update()
    {
        if (shakes.Count > 0)
        {
            Shake greatestShake = shakes[0];
            for (var i = 0; i < shakes.Count; i++)
            {
                shakes[i].elapsed += Time.unscaledDeltaTime;
                if (shakes[i].magnitude > greatestShake.magnitude)
                {
                    greatestShake = shakes[i];
                }
            }
            float x = Random.Range(-1f, 1f) * greatestShake.magnitude;
            float y = Random.Range(-1f, 1f) * greatestShake.magnitude;
            transform.localPosition = new Vector3(x,y,transform.localPosition.z);
            for (var i = shakes.Count - 1; i >= 0; i--)
            {
                if (shakes[i].elapsed >= shakes[i].duration)
                {
                    shakes.Remove(shakes[i]);
                }
            }
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public IEnumerator Shake(float duration, float magnitude, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x,y,originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }
    
    public IEnumerator Shake2(float duration, float magnitude, IEnumerator toStart)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x,y,originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        if (toStart != null)
        {
            StartCoroutine(toStart);
        }
    }
    
    public IEnumerator StopShakeOverTime(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;
        float currentMagnitude = magnitude;

        while (elapsed < duration)
        {
            currentMagnitude = ((duration - elapsed) / duration) * magnitude;
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;
            transform.localPosition = new Vector3(x,y,originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }
    
    public IEnumerator ShakeOverTime(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;
        float currentMagnitude = 0f;

        while (elapsed < duration)
        {
            currentMagnitude = (elapsed / duration) * magnitude;
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;
            transform.localPosition = new Vector3(x,y,originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }
}

public class Shake
{
    public float duration;
    public float magnitude;
    public float elapsed;

    public Shake(float dur, float mag)
    {
        duration = dur;
        magnitude = mag;
    }
}
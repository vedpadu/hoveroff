using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handScript : MonoBehaviour
{
    public AnimationCurve punchCurve;

    public Transform target;

    public float punchTimePerUnit;

    private bool init;

    public float punchEndShakeMag;

    public float punchEndShakeDur;

    private Camera cam;

    private cameraShake camerShake;
    
    [Range(0f,1f)] public float punchShakeStart;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        camerShake = cam.GetComponent<cameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            float dist = ((Vector2)target.position - (Vector2)transform.position).magnitude;
            float time = punchTimePerUnit * dist;
            StartCoroutine(Punch(time, target.position, punchCurve, transform.position));
            init = true;
        }
    }

    IEnumerator Punch(float duration, Vector2 target, AnimationCurve punchCurveLocal, Vector2 startPos)
    {
        yield return new WaitForSeconds(0.6f);
        float elapsed = 0.0f;
        bool shakeStarted = false;
        while (elapsed < duration)
        {
            float lerpVal = punchCurveLocal.Evaluate(elapsed / duration);
            transform.position = CustomVector2Lerp(startPos, target, lerpVal);
            if (!shakeStarted && elapsed/duration > punchShakeStart)
            {
                camerShake.shakes.Add(new Shake(punchEndShakeDur, punchEndShakeMag));
                shakeStarted = true;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        
    }

    public Vector2 CustomVector2Lerp(Vector2 a, Vector2 b, float t)
    {
        Vector2 dir = (b - a);
        float dist = dir.magnitude;
        dist = dist * (t / 1);
        return a + (dir.normalized * dist);
    }
}

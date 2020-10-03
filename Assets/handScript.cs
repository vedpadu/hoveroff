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

    public AnimationCurve rotateToTargetCurve;

    public float timePerRotation;

    public AnimationCurve moveToPunchStartCurve;
    public float moveToPunchStartTimePerUnit;

    private bool readyToPunch;
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
            StartCoroutine(GoToPunchStart());
            init = true;
        }

        if (readyToPunch)
        {
            print(readyToPunch);
            float dist = ((Vector2)target.GetChild(0).position - (Vector2)transform.position).magnitude;
            float time = punchTimePerUnit * Mathf.Abs(dist);
            StartCoroutine(Punch(time, target.GetChild(0).position, punchCurve, transform.position));
            readyToPunch = false;
        }
    }

    IEnumerator GoToPunchStart()
    {
        Vector2 dir = target.position - transform.position;
        float dist = dir.magnitude;
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float startAngle = transform.rotation.eulerAngles.z;
        float delta = Mathf.DeltaAngle(startAngle, angle);
        float timeToTurnToTarget = Mathf.Abs(delta) * (timePerRotation / 360f);
        float elapsed1 = 0.0f;
        while (elapsed1 < timeToTurnToTarget)
        {
            float lerpVal = rotateToTargetCurve.Evaluate(elapsed1 / timeToTurnToTarget);
            transform.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(startAngle,angle,lerpVal));
            elapsed1 += Time.deltaTime;
            yield return null;
        }

        float elapsed2 = 0.0f;
        float timeToMoveToTarget = Mathf.Abs(dist) * moveToPunchStartTimePerUnit;
        Vector2 startPos = transform.position;
        while (elapsed2 < timeToMoveToTarget)
        {
            float lerpVal = moveToPunchStartCurve.Evaluate(elapsed2 / timeToMoveToTarget);
            transform.position = CustomVector2Lerp(startPos, target.position, lerpVal);
            elapsed2 += Time.deltaTime;
            yield return null;
        }

        float elapsed3 = 0.0f;
        startAngle = transform.rotation.eulerAngles.z;
        delta = Mathf.DeltaAngle(startAngle, target.rotation.eulerAngles.z);
        timeToTurnToTarget = Mathf.Abs(delta) * (timePerRotation / 360f);
        while (elapsed3 < timeToTurnToTarget)
        {
            float lerpVal = rotateToTargetCurve.Evaluate(elapsed3 / timeToTurnToTarget);
            transform.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(startAngle,target.rotation.eulerAngles.z,lerpVal));
            elapsed3 += Time.deltaTime;
            yield return null;
        }

        readyToPunch = true;
    }

    IEnumerator Punch(float duration, Vector2 targetPos, AnimationCurve punchCurveLocal, Vector2 startPos)
    {
        yield return new WaitForSeconds(0.6f);
        float elapsed = 0.0f;
        bool shakeStarted = false;
        while (elapsed < duration)
        {
            float lerpVal = punchCurveLocal.Evaluate(elapsed / duration);
            transform.position = CustomVector2Lerp(startPos, targetPos, lerpVal);
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

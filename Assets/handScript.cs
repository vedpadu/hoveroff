using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handScript : MonoBehaviour
{
    public AnimationCurve punchCurve;

    public Transform target;

    public float punchTimePerUnit;

    public float timeBeforePunchColorFade;
    public float timeStunnedPunch;

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

    [HideInInspector]
    public bool readyToPunch;

    private Color colorNotPunch;

    private SpriteRenderer sR;
    public float notPunchScale;
    float scaleVal;
    private Material srMat;

    [HideInInspector]
    public bool goToTarget;

    public ParticleSystem engines;

    [HideInInspector]
    public bool punched;

    public bool lockDown;

    public bool isLeftHand;

    public float timeBeforePunch = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        srMat = sR.material;
        colorNotPunch = GetComponent<SpriteRenderer>().color;
        cam = Camera.main;
        camerShake = cam.GetComponent<cameraShake>();
        scaleVal = notPunchScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (goToTarget)
        {
            if (!lockDown)
            {
                StartCoroutine(GoToPunchStart());
                goToTarget = false;
            }

           
        }

        /*if (readyToPunch)
        {
            float dist = ((Vector2)target.GetChild(0).position - (Vector2)transform.position).magnitude;
            float time = punchTimePerUnit * Mathf.Abs(dist);
            StartCoroutine(Punch(time, target.GetChild(0).position, punchCurve, transform.position, timeBeforePunchColorFade, timeStunnedPunch));
            readyToPunch = false;
        }*/
    }

    public void Punch()
    {
        if (!lockDown)
        {
            float dist = ((Vector2)target.GetChild(0).position - (Vector2)transform.position).magnitude;
            float time = punchTimePerUnit * Mathf.Abs(dist);
            StartCoroutine(Punch(time, target.GetChild(0).position, punchCurve, transform.position, timeBeforePunchColorFade, timeStunnedPunch, timeBeforePunch));
            readyToPunch = false;
        }
       
    }

    IEnumerator GoToPunchStart()
    {
        Vector2 dir = target.position - transform.position;
        float dist = dir.magnitude;
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (isLeftHand)
        {
            angle += 180f;
        }
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

    IEnumerator Punch(float duration, Vector2 targetPos, AnimationCurve punchCurveLocal, Vector2 startPos, float colorFadeTime, float timeStunned, float timeBeforeStart)
    {
        yield return new WaitForSeconds(timeBeforeStart);
       
        float elapsed1 = 0.0f;
        while (elapsed1 < colorFadeTime)
        {
            if (!this.Equals(null))
            {
                srMat.SetFloat("_Alpha", 1 - (elapsed1/colorFadeTime));
                sR.color = Color.Lerp(colorNotPunch, Color.white, 1 - (elapsed1 / colorFadeTime));
            }
            elapsed1 += Time.deltaTime;
            yield return null;
        }
        GetComponent<PolygonCollider2D>().enabled = true;
        

        float elapsed = 0.0f;
        bool shakeStarted = false;
        bool partsStarted = false;
        bool partsEnded = false;
        while (elapsed < duration)
        {
            if(!this.Equals(null)){
                float lerpVal = punchCurveLocal.Evaluate(elapsed / duration);
                transform.position = CustomVector2Lerp(startPos, targetPos, lerpVal);
                if (elapsed / duration > 0.6f && !partsStarted)
                {
                    engines.Play();
                    partsStarted = true;
                }
                if (!shakeStarted && elapsed/duration > punchShakeStart)
                {
                    camerShake.shakes.Add(new Shake(punchEndShakeDur, punchEndShakeMag));
                    shakeStarted = true;
                    
                }

                if (!partsEnded && elapsed / duration > 0.9f)
                {
                    engines.Stop();
                    partsEnded = true;
                }
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(timeStunnedPunch);

        elapsed1 = 0.0f;
        while (elapsed1 < colorFadeTime)
        {
            if (!this.Equals(null))
            {
                srMat.SetFloat("_Alpha", elapsed1/colorFadeTime);
                sR.color = Color.Lerp(Color.white, colorNotPunch, elapsed1 / colorFadeTime);
            }
            
            elapsed1 += Time.deltaTime;
            yield return null;
        }

        if (!this.Equals(null))
        {
             GetComponent<PolygonCollider2D>().enabled = false;
        }
       
        punched = true;

    }

    public Vector2 CustomVector2Lerp(Vector2 a, Vector2 b, float t)
    {
        Vector2 dir = (b - a);
        float dist = dir.magnitude;
        dist = dist * (t / 1);
        return a + (dir.normalized * dist);
    }
}

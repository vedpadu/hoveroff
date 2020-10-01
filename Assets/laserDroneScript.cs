using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserDroneScript : MonoBehaviour
{
    public Transform target;

    public float angleLerp;

    public float movementForce;

    private Rigidbody2D rb;

    public float slowDownDistance;

    private float slowDownTime;
    

    private bool slowDownStarted;

    private float startSlowDownVel;

    private float slowDownAcceleration;

    private float slowDownTimer;

    [HideInInspector]
    public bool reachedTarget;

    private Vector2 oldTargPos;

    private Vector2 slowDownDir;
    
    float endAngleSlowDown;
    private commonEnemyBehaviour cEB;
    private float cEBAvoidanceLerp;

    public float hue;

    public SpriteRenderer coloredParts;
    public LineRenderer laser;
    public GameObject preLaser;
    private TrailRenderer tR;
    [HideInInspector]
    public bool isSpinning;

    [HideInInspector]
    public bool secondStageSwitchTarget;
    [HideInInspector]public bool secondStageInCoroutine;

    [HideInInspector]
    public bool DoTargetting;

    [HideInInspector] public bool stage2;

    private Shake laserShake = null;

    private cameraShake camerShake;

    public float secondStageShakeMag;
    // Start is called before the first frame update
    void Start()
    {
        tR = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        cEB = GetComponent<commonEnemyBehaviour>();
        cEBAvoidanceLerp = cEB.avoidanceLerp;
        camerShake = Camera.main.GetComponent<cameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        coloredParts.color = Color.HSVToRGB(hue, 0.39f, 1f);
        laser.startColor = Color.HSVToRGB(hue, 0.39f, 1f);
        laser.endColor = Color.HSVToRGB(hue, 0.39f, 1f);
        tR.startColor = Color.HSVToRGB(hue, 0.39f, 1f);
        tR.endColor =  Color.HSVToRGB(hue, 0.39f, 1f);
        if (stage2)
        {
            if (laser.gameObject.activeSelf)
            {
                if (!camerShake.shakes.Contains(laserShake))
                {
                    laserShake = new Shake(10000000f, secondStageShakeMag);
                    camerShake.shakes.Add(laserShake);
                }
            }
            else
            {
                if (camerShake.shakes.Contains(laserShake))
                {
                    camerShake.shakes.Remove(laserShake);
                    laserShake = null;
                }
            }
        }
        CalculateAngle();
        DoMovement();
        if (reachedTarget)
        {
            if (!tR.enabled)
            {
                tR.enabled = true;
            }
            transform.position = target.position;
            if (((Vector2)oldTargPos - (Vector2)target.position).sqrMagnitude > 0.5f)
            {
                reachedTarget = false;
                slowDownStarted = false;
            }

            rb.isKinematic = true;
            cEB.avoidanceLerp = 0f;
        }
        else
        {
/*            if (tR.enabled)
            {
                tR.enabled = false;
            }*/
            rb.isKinematic = false;
            cEB.avoidanceLerp = cEBAvoidanceLerp;
        }
        oldTargPos = target.position;
    }

    private void OnDestroy()
    {
        if (camerShake.shakes.Contains(laserShake))
        {
            camerShake.shakes.Remove(laserShake);
        }
    }

    void CalculateAngle()
    {
        if (Vector2.Distance(target.position, transform.position) > slowDownDistance)
        {
            reachedTarget = false;
            Vector2 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, angleLerp * Time.deltaTime));
        }
        else
        {
            if (!slowDownStarted)
            {
                endAngleSlowDown = target.transform.rotation.eulerAngles.z;
            }

            if (!isSpinning)
            {
                transform.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(transform.rotation.eulerAngles.z, target.rotation.eulerAngles.z, angleLerp * Time.deltaTime));
            }
               

            }
    }

    void DoMovement()
    {
        float dist = Vector2.Distance(target.position, transform.position);
        if (dist > slowDownDistance)
        {
            rb.AddRelativeForce(new Vector2(movementForce * Time.deltaTime, 0f));
            slowDownStarted = false;
        }
        else if(!reachedTarget)
        {
            if (!slowDownStarted)
            {
                slowDownDir = (target.position - transform.position).normalized;
                if (Mathf.Approximately(rb.velocity.magnitude, 0f))
                {
                    rb.velocity = slowDownDir * 5f;
                }

                if (Mathf.Approximately(slowDownDistance, 0f))
                {
                    slowDownTimer = slowDownTime;
                    reachedTarget = true;
                    transform.position = target.position;
                }
                
                startSlowDownVel = Mathf.Abs(rb.velocity.magnitude);
                slowDownTime = (2f * slowDownDistance) / startSlowDownVel;
                slowDownTimer = ((slowDownDistance - dist) / slowDownDistance) * slowDownTime;
                slowDownAcceleration = Mathf.Pow(startSlowDownVel, 2f) / (2 * -slowDownDistance);
                slowDownStarted = true;
                rb.velocity = Vector2.zero;
            }

            slowDownTimer += Time.deltaTime;
            if (slowDownTimer >= slowDownTime)
            {
                //print("reached");
                slowDownTimer = slowDownTime;
                reachedTarget = true;
                transform.position = target.position;
            }

            float pos = -slowDownDistance + (startSlowDownVel * slowDownTimer) +
                        (0.5f * slowDownAcceleration * Mathf.Pow(slowDownTimer, 2f));
            if (!float.IsNaN(pos))
            {
                transform.position = (Vector2)target.position + (slowDownDir * pos);
            }
            //print(slowDownTimer + " " + pos);
            


        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class helicarrierHiveScript : MonoBehaviour
{
    public float circlingLerp;
    
    
    private cameraShake camerShake;
    private Rigidbody2D rb;

    private bool circling;
    public float torqueAmount;

    public Transform target;

    public float moveForce;

    public GameObject hivePathing;

    private List<Transform> pathPoints = new List<Transform>();

    private int currentPathPoint;

    public float pathIncrementDistance;

    public Transform head;

    public Transform sprite;

    public GameObject shockwave;

    public GameObject hiveShip;

    public float shipLaunchVelocity;

    public float shipLaunchTime;

    private List<float> shipLaunchTimers = new List<float>();
    private List<Transform> shipsLaunching = new List<Transform>();
    private List<bool> hasLaunched = new List<bool>();

    public float timeBetweenLaunches;

    public float timeBetweenWaves;

    public int shipsPerWave;

    private float waveTimer;

    private bool launching;

    public Transform instantiationPoint;

    public healthScript hS;

    [Range(0f,1f)]
    public float whenToStartSecondStage;

    private bool secondStage;

    private bool secondStageJustStarted;
    bool hasDoneSecondStageCoroutine;

    public AnimationCurve velocityDecreaseCurve;

    public float hoverDistance;

    public float velocityStopDur;

    public float hoverStopDirectionDist;

    private Vector2 currentHoverPosition;

    public float hoverLerp;
    public float secondStageAngularAcceleration;

    private float secondStageAngularVelocity;

    public float maxAngularVelocitySecondStage;

    private float currentSecondStageAngle;

    public List<GameObject> lasers;

    [Range(0f,1f)]public float secondStageEasinessValue;

    public GameObject secondWaveHiveShip;

    private Shake laserShake;
    // Start is called before the first frame update
    void Start()
    {
        GameObject pathing = GameObject.Instantiate(hivePathing, Vector3.zero, Quaternion.identity);
        for (var i = 0; i < pathing.transform.childCount; i++)
        {
            pathPoints.Add(pathing.transform.GetChild(i));
        }

        waveTimer = timeBetweenWaves;

        int bestPathPoint = 0;
        float distBest = Vector2.Distance(pathPoints[bestPathPoint].position, head.position);
        for (var i = 1; i < pathPoints.Count; i++)
        {
            float newDist = Vector2.Distance(pathPoints[i].position, head.position);
            if (newDist < distBest)
            {
                bestPathPoint = i;
                distBest = newDist;
            }
        }

        currentPathPoint = bestPathPoint;
        camerShake = Camera.main.GetComponent<cameraShake>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!secondStage)
        {
            if (hS.health <= hS.maxHealth * whenToStartSecondStage)
            {
                secondStage = true;
                secondStageJustStarted = true;
            }
        }

        DoWaves();
        CalculateAngle();
        DoMovement();
        
    }

    private void LateUpdate()
    {
        if (!secondStage || secondStageJustStarted)
        {
            Vector2 velocity = rb.velocity.normalized;
            float velAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            sprite.rotation = Quaternion.Euler(0f,0f,velAngle);
            currentSecondStageAngle = velAngle;
        }
        else
        {
            if (secondStageAngularVelocity < maxAngularVelocitySecondStage)
            {
                secondStageAngularVelocity += secondStageAngularAcceleration * Time.deltaTime;
            }
            else
            {
                secondStageAngularVelocity = maxAngularVelocitySecondStage;
            }
            
            currentSecondStageAngle -= secondStageAngularVelocity * Time.deltaTime;
            if (currentSecondStageAngle <= -360f)
            {
                currentSecondStageAngle += 360f;
            }
            sprite.rotation = Quaternion.Euler(0f,0f,currentSecondStageAngle);
        }
       
    }

    void DoMovement()
    {
        rb.AddRelativeForce(new Vector2(moveForce * Time.deltaTime, 0f));
        if (secondStageJustStarted)
        {
            if (Vector2.Distance(Vector2.zero, transform.position) <= hoverDistance)
            {
                secondStageJustStarted = false;
                foreach (GameObject laser in lasers)
                {
                    laser.SetActive(true);
                }

                timeBetweenLaunches /= secondStageEasinessValue;
                shipsPerWave = Mathf.RoundToInt(shipsPerWave * secondStageEasinessValue);
                laserShake = new Shake(1000000000000f, 0.025f);
                camerShake.shakes.Add(laserShake);
            }
        }
        /*if (!secondStage)
        {
            
        }
        else
        {
            if (secondStageJustStarted)
            {
                if (Vector2.Distance(Vector2.zero, transform.position) <= hoverDistance)
                {
                    if (!hasDoneSecondStageCoroutine)
                    {
                        StartCoroutine(ReduceVel(velocityStopDur, velocityDecreaseCurve));
                        hasDoneSecondStageCoroutine = true;
                    }
                    float currentHoverAngle = Random.Range(0f, 360f);
                    currentHoverAngle *= Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(Mathf.Cos(currentHoverAngle), Mathf.Sin(currentHoverAngle));
                    dir *= hoverDistance;
                    currentHoverPosition = dir;
                    secondStageJustStarted = false;

                }
                else if(!hasDoneSecondStageCoroutine)
                {
                    rb.AddRelativeForce(new Vector2(moveForce, 0f));
                }
            }
            else
            {
                if (Vector2.Distance(currentHoverPosition, transform.position) <= hoverStopDirectionDist)
                {
                    float currentHoverAngle = Random.Range(0f, 360f);
                    currentHoverAngle *= Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(Mathf.Cos(currentHoverAngle), Mathf.Sin(currentHoverAngle));
                    dir *= hoverDistance;
                    currentHoverPosition = dir;
                }
                else
                {
                    Vector2 pos = transform.position;
                    pos = Vector2.Lerp(pos, currentHoverPosition, hoverLerp);
                    transform.position = new Vector3(pos.x, pos.y, transform.position.z);
                }
            }
        }*/
    }

    void CalculateAngle()
    {
        Vector2 targetPos = Vector2.zero;
        if (!secondStage)
        {
            if (Vector2.Distance(pathPoints[currentPathPoint].position, head.position) < pathIncrementDistance)
            {
                if (currentPathPoint >= pathPoints.Count - 1)
                {
                    currentPathPoint = 0;
                }
                else
                {
                    currentPathPoint++;
                }
            }
            targetPos = pathPoints[currentPathPoint].position;
        }
        else
        {
            if(secondStageJustStarted)
            {
                targetPos = Vector2.zero;
            }
            else
            {
                targetPos = Vector2.zero;
            }
        }
        
            Vector2 dir = targetPos - (Vector2) transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, circlingLerp);
            transform.rotation = Quaternion.Euler(0f,0f,angle);
            
      
       
        


    }

    void SpawnWave()
    {
        shipLaunchTimers.Clear();
        shipsLaunching.Clear();
        hasLaunched.Clear();
        for (var i = 0; i < shipsPerWave; i++)
        {
            if (secondStage)
            {
                shipsLaunching.Add(GameObject.Instantiate(secondWaveHiveShip, instantiationPoint).transform);
            }
            else
            {
                shipsLaunching.Add(GameObject.Instantiate(hiveShip, instantiationPoint).transform);
            }
           
            shipLaunchTimers.Add(0f);
            hasLaunched.Add(false);
        }
    }

    void DoWaves()
    {
        if (!launching)
        {
            waveTimer += Time.deltaTime;
            if (waveTimer > timeBetweenWaves)
            {
                waveTimer = 0f;
                launching = true;
                SpawnWave();
            }
        }
        else
        {
            for (var i = 0; i < shipsLaunching.Count; i++)
            {
                if (i == 0)
                {
                    shipLaunchTimers[i] += Time.deltaTime;
                }
                else
                {
                    if (shipLaunchTimers[i - 1] >= timeBetweenLaunches)
                    {
                        shipLaunchTimers[i] += Time.deltaTime;
                    }
                }

                if (shipLaunchTimers[i] < shipLaunchTime)
                {
                    shipsLaunching[i].localPosition = new Vector3(shipLaunchTimers[i] * shipLaunchVelocity, 0f, 0f);
                }
                else
                {
                    if (!hasLaunched[i])
                    {
                        shipsLaunching[i].GetComponent<commonEnemyBehaviour>().enabled = true;
                        kamikazeEnemyScript kES = shipsLaunching[i].GetComponent<kamikazeEnemyScript>();
                        kES.enabled = true;
                        if (target != null)
                        {
                            kES.target = target;
                        }
                        
                        Rigidbody2D shipLaunching = shipsLaunching[i].GetComponent<Rigidbody2D>();
                        shipLaunching.simulated = true;
                        shipsLaunching[i].parent = null;
                        float angle = shipsLaunching[i].rotation.eulerAngles.z * Mathf.Deg2Rad;
                        shipLaunching.velocity = shipLaunchVelocity * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                        hasLaunched[i] = true;
                    }
                }
               
            }

            if (hasLaunched[shipsLaunching.Count - 1])
            {
                launching = false;
            }
        }
    }

    IEnumerator ReduceVel(float duration, AnimationCurve curve)
    {

        float elapsed = 0.0f;
        Vector2 currentVel = rb.velocity;
        Vector2 origVel = rb.velocity;
        while (elapsed < duration)
        {
            rb.velocity = curve.Evaluate(elapsed / duration) * origVel;
            elapsed += Time.deltaTime;
            yield return null;
        }

        //rb.simulated = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        secondStageJustStarted = false;
    }

    public void OnDestroy()
    {
        camerShake.shakes.Remove(laserShake);
    }
}

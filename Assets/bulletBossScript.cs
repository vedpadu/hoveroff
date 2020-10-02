using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBossScript : MonoBehaviour
{
    public AnimationCurve moveCurve;

    public float movementTimePerUnit;

    public float rotationVelocity;

    public float rotationAcceleration;

    public float timeBeforeMovement;

    public float attackingTime;

    private float attackingTimer;

    private bool isAttacking;

    private bool attackingJustChanged = true;

    private Camera cam;
    public List<Transform> bulletTransforms1;
    public List<Transform> bulletTransforms2;
    public List<GameObject> lasers1;
    public List<GameObject> lasers2;
    public float bulletForce;

    public GameObject bullet;

    private healthScript hS;

    private int currentAttack = 0;

    private bool startSpawningEnemies;

    public GameObject enemyToSpawn;

    public float spawnInterval;

    private float spawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnInterval;
        hS = GetComponent<healthScript>();
        cam = Camera.main;
        currentAttack = Random.Range(0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        ManageStates();
        if (startSpawningEnemies)
        {
            if (spawnTimer >= spawnInterval)
            {
                Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)).normalized;
                dir *= 20f;
                GameObject.Instantiate(enemyToSpawn, dir, Quaternion.identity);
                spawnTimer = 0f;

            }
            spawnTimer += Time.deltaTime;
        }
    }

    void ManageStates()
    {
        if (hS.health < hS.maxHealth / 2f)
        {
            startSpawningEnemies = true;
        }
        if (isAttacking)
        {
            if (attackingJustChanged)
            {
                if (currentAttack == 0)
                {
                    StartCoroutine(DoSpinAttack(360f, 3f, 4f));
                }else if (currentAttack == 1)
                {
                    StartCoroutine(laserShootingAttack(15f, 4f, 3f));
                }
                //StartCoroutine(DoSpinAttack(200f, 3f, 10f));
                //StartCoroutine(laserShootingAttack(15f, 10f, 3f));
                attackingJustChanged = false;
            }
            /*attackingTimer += Time.deltaTime;
            if (attackingTimer >= attackingTime)
            {
                attackingTimer = 0f;
                isAttacking = false;
                attackingJustChanged = true;
            }*/
        }
        else
        {
            if (attackingJustChanged)
            {
                Vector2 endPoint = Vector2.zero;
                if (currentAttack == 0)
                {
                    endPoint = new Vector2(Random.Range(-cam.orthographicSize * (Screen.width/(float)Screen.height), cam.orthographicSize * (Screen.width/(float)Screen.height)), Random.Range(-cam.orthographicSize, cam.orthographicSize));
                }else if (currentAttack == 1)
                {
                    endPoint = Vector2.zero;
                }
                Vector2 startPoint = transform.position;
                float dist = (endPoint - startPoint).magnitude;
                float duration = dist * movementTimePerUnit;
                StartCoroutine(DoCurrentMovement(endPoint, startPoint, duration));
                attackingJustChanged = false;
            }
        }
    }

    IEnumerator DoCurrentMovement(Vector2 endPoint, Vector2 startPoint, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float lerpVal = moveCurve.Evaluate(elapsed / duration);
            transform.position = Vector2.Lerp(startPoint, endPoint, lerpVal);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isAttacking = true;
        attackingJustChanged = true;
    }

    IEnumerator DoSpinAttack(float endRotationVelocity, float fireRate, float time)
    {
        float elapsed = 0.0f;
        float shootTime = 0.0f;
        while (elapsed < time)
        {
            if (rotationVelocity < endRotationVelocity)
            {
                rotationVelocity += rotationAcceleration * Time.deltaTime;
            }

            if (shootTime > 1f / fireRate)
            {
                for (var i = 0; i < bulletTransforms1.Count; i++)
                {
                    Vector2 dir = (bulletTransforms1[i].position - transform.position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    GameObject bulletObj = GameObject.Instantiate(bullet, bulletTransforms1[i].position, Quaternion.Euler(0f,0f,angle));
                    bulletObj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(bulletForce,0f));
                    bulletObj.GetComponent<bulletScript>().shooter = hS;
                    //Destroy(GameObject.Instantiate(shootBulletParticles, shootParticlesTransform.position, transform.rotation),3f);
                    //camerShake.shakes.Add(new Shake(0.1f, 0.1f));
                }
                for (var i = 0; i < bulletTransforms2.Count; i++)
                {
                    Vector2 dir = (bulletTransforms2[i].position - transform.position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    GameObject bulletObj = GameObject.Instantiate(bullet, bulletTransforms2[i].position, Quaternion.Euler(0f,0f,angle));
                    bulletObj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(bulletForce,0f));
                    bulletObj.GetComponent<bulletScript>().shooter = hS;
                    //Destroy(GameObject.Instantiate(shootBulletParticles, shootParticlesTransform.position, transform.rotation),3f);
                    //camerShake.shakes.Add(new Shake(0.1f, 0.1f));
                }
                shootTime = 0.0f;
            }
            transform.Rotate(0f,0f, rotationVelocity * Time.deltaTime);
            elapsed += Time.deltaTime;
            shootTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        attackingJustChanged = true;
        currentAttack += 1;
        if (currentAttack > 1)
        {
            currentAttack = 0;
        }
    }

    IEnumerator laserShootingAttack(float endRotationVelocity, float duration, float fireRate)
    {
        rotationVelocity = 0f;
        float elapsed = 0.0f;
        float shootTime = 0.0f;
        for (var i = 0; i < lasers1.Count; i++)
        { 
            lasers1[i].SetActive(true);
        }
        while (elapsed < duration)
        {
            if (rotationVelocity < endRotationVelocity)
            {
                rotationVelocity += rotationAcceleration * Time.deltaTime;
            }

            if (shootTime > 1f / fireRate)
            {
                for (var i = 0; i < bulletTransforms2.Count; i++)
                {
                    Vector2 dir = (bulletTransforms2[i].position - transform.position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    GameObject bulletObj = GameObject.Instantiate(bullet, bulletTransforms2[i].position, Quaternion.Euler(0f,0f,angle));
                    bulletObj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(bulletForce,0f));
                    bulletObj.GetComponent<bulletScript>().shooter = hS;
                    //Destroy(GameObject.Instantiate(shootBulletParticles, shootParticlesTransform.position, transform.rotation),3f);
                    //camerShake.shakes.Add(new Shake(0.1f, 0.1f));
                }
                shootTime = 0.0f;
            }
            transform.Rotate(0f,0f, rotationVelocity * Time.deltaTime);
            elapsed += Time.deltaTime;
            shootTime += Time.deltaTime;
            yield return null;
        }
        for (var i = 0; i < lasers1.Count; i++)
        { 
            lasers1[i].SetActive(false);
        }
        isAttacking = false;
        attackingJustChanged = true;
        currentAttack += 1;
        if (currentAttack > 1)
        {
            currentAttack = 0;
        }
    }
}

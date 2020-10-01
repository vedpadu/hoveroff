using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingEnemyScript : MonoBehaviour
{
    public Transform target;

    public float angleLerp;

    public float moveForce;

    private Rigidbody2D rb;

    private cameraShake camerShake;

    public float distanceMoveToward;

    public float distanceMoveAway;

    private bool movingTowards = true;

    public GameObject bullet;

    public Transform bulletShootArea;

    public float bulletShootDistance;

    private bool hasShotBullet = true;

    public float bulletForce;

    public GameObject shootBulletParticles;
    public Transform shootParticlesTransform;

    private healthScript hS;
    // Start is called before the first frame update
    void Start()
    {
        hS = GetComponent<healthScript>();
        camerShake = Camera.main.GetComponent<cameraShake>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateAngle();
        rb.AddRelativeForce(new Vector2(moveForce, 0f));
    }
    
    void CalculateAngle()
    {
        Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angle = transform.rotation.eulerAngles.z;

        float dist = dir.magnitude;
        /*float key = dist / distanceRange;
        float weightToward = curveToward.Evaluate(key);
        float weightAway = curveAway.Evaluate(key);*/
        
        float angleToward = Mathf.LerpAngle(angle, targetAngle, angleLerp);
        float angleAway = Mathf.LerpAngle(angle, targetAngle + 180f, angleLerp);
        if (movingTowards)
        {
            if (!hasShotBullet)
            {
                if (dist <= bulletShootDistance)
                {
                    GameObject bulletObj = GameObject.Instantiate(bullet, bulletShootArea.position, transform.rotation);
                    bulletObj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(bulletForce,0f));
                    bulletObj.GetComponent<bulletScript>().shooter = hS;
                    hasShotBullet = true;
                    Destroy(GameObject.Instantiate(shootBulletParticles, shootParticlesTransform.position, transform.rotation),3f);
                    camerShake.shakes.Add(new Shake(0.1f, 0.1f));
                    //StartCoroutine(camerShake.Shake(0.1f, 0.1f, 0f));
                }
            }
            if (dist > distanceMoveAway)
            {
                angle = angleToward;
            }
            else
            {
                movingTowards = false;
                angle = angleAway;
            }
        }
        else
        {
            if (dist < distanceMoveToward)
            {
                angle = angleAway;
            }
            else
            {
                angle = angleToward;
                movingTowards = true;
                hasShotBullet = false;
            }
        }
        /*angle = (angleToward * weightToward) + (angleAway * weightAway);
        angle /= (weightAway + weightToward);*/
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
       /* if (Mathf.Abs(Mathf.DeltaAngle(angle, targetAngle)) < 5f)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }*/
    }
    
}

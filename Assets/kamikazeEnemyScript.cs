using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kamikazeEnemyScript : MonoBehaviour
{
    public Transform target;

    public float angleLerp;

    public float moveForce;

    private Rigidbody2D rb;
    

    private cameraShake camerShake;

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
        rb.AddRelativeForce(new Vector2(moveForce * Time.deltaTime, 0f));
    }
    
    void CalculateAngle()
    {
        Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angle = transform.rotation.eulerAngles.z;


        angle = Mathf.LerpAngle(angle, targetAngle, angleLerp * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
}

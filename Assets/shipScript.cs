using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipScript : MonoBehaviour
{
    public float angleLerp;

    private Camera mainCam;

    public float moveForce;

    private Rigidbody2D rb;

    public GameObject laser;

    private laserScript laserScript;

    private cameraShake camerShake;

    public float laserShakeMagnitude;

    public float laserRecoilForce;

    public Transform camerHolder;

    private Shake cameraShakeLaser;
    public float strafeForce;

    private healthScript hS;
    // Start is called before the first frame update
    void Start()
    {
        hS = GetComponent<healthScript>();
        mainCam = Camera.main;
        camerShake = mainCam.GetComponent<cameraShake>();
        rb = GetComponent<Rigidbody2D>();
        laserScript = laser.GetComponent<laserScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateAngle();

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(new Vector2(moveForce * Time.deltaTime, 0f));
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddRelativeForce(new Vector2(0f, (strafeForce/2) * Time.deltaTime));
            }
        
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddRelativeForce(new Vector2(0f, (-strafeForce/2) * Time.deltaTime));
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddRelativeForce(new Vector2(0f, strafeForce * Time.deltaTime));
            }
        
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddRelativeForce(new Vector2(0f, -strafeForce * Time.deltaTime));
            }
        }

        

        if (Input.GetMouseButtonDown(0))
        {
            laserScript.currentLaserDist = 0f;
            //cameraShakeLaser = StartCoroutine(camerShake.Shake(10000000f, laserShakeMagnitude, 0f));
            cameraShakeLaser = new Shake(1000000f, laserShakeMagnitude);
            camerShake.shakes.Add(cameraShakeLaser);
            laser.SetActive(true);
            
        }

        if (Input.GetMouseButton(0))
        {
            rb.AddRelativeForce(new Vector2(-laserRecoilForce * Time.deltaTime, 0f));
        }

        if (Input.GetMouseButtonUp(0))
        {
            camerShake.shakes.Remove(cameraShakeLaser);
            //StopCoroutine(cameraShakeLaser);
            cameraShakeLaser = null;
            mainCam.transform.localPosition = Vector3.zero;
            laser.SetActive(false);
            
        }
        //camerHolder.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    void CalculateAngle()
    {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angle = transform.rotation.eulerAngles.z;


        angle = Mathf.LerpAngle(angle, targetAngle, angleLerp * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ShakeCam2(float duration, float mag)
    {
        camerShake.shakes.Add(new Shake(duration, mag));
        //StartCoroutine(camerShake.Shake2(duration, mag, null));

    }

    public void StopShakeCamOverTime(float duration, float mag)
    {
        StartCoroutine(camerShake.StopShakeOverTime(duration, mag));
    }
}

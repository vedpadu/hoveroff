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
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ManageStates();
    }

    void ManageStates()
    {
        if (isAttacking)
        {
            if (attackingJustChanged)
            {
                attackingJustChanged = false;
            }
            attackingTimer += Time.deltaTime;
            if (attackingTimer >= attackingTime)
            {
                attackingTimer = 0f;
                isAttacking = false;
                attackingJustChanged = true;
            }
        }
        else
        {
            if (attackingJustChanged)
            {
                Vector2 endPoint = new Vector2(Random.Range(-cam.orthographicSize * (Screen.width/(float)Screen.height), cam.orthographicSize * (Screen.width/(float)Screen.height)), Random.Range(-cam.orthographicSize, cam.orthographicSize)); 
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
}

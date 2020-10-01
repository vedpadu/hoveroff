using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashingBossScript : MonoBehaviour
{
    public Transform target;
    private Vector2 targettedPoint;
    private Vector2 targetLRPoint;
    public Color targetingColorLine;
    public Color attackingColorLine;
    public float colorFlashTime;
    public float targetingLerp;
    public float targetingTime;
    public float timeTillDashStart;
    private float timerTillDashStart;
    private float targetingTimer;
    private bool dashing;
    public Transform lRStart;
    
    public float dashVelocity;
    public float distanceToVelZero;
    [Range(0f,1f)]
    public float threshholdToVelZero;

    [Range(0f, 1f)] public float velocityLerpToZero;    
    
    public LineRenderer targetingLine;
    public float targetingLineWidth;
    Vector3[] lRPoints = new Vector3[2];

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetingLine.startWidth = targetingLineWidth;
        targetingLine.endWidth = targetingLineWidth;
        targetingLine.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        lRPoints[0] = (Vector2) lRStart.position;
        lRPoints[1] = targetLRPoint;
        targetingLine.SetPositions(lRPoints);
        if (!dashing)
        {
            Targeting();
        }
        else
        {
            Dashing();
        }
        
    }

    private void Targeting()
    {
        targetingLine.startColor = targetingColorLine;
        targetingLine.endColor = targetingColorLine;
        if (targetingTime - targetingTimer < targetingTime * 0.3f)
        {
            if (Mathf.RoundToInt(targetingTimer * 1/colorFlashTime) % 2 == 0)
            {
                targetingLine.startColor = attackingColorLine;
                targetingLine.endColor = attackingColorLine;
            }
        }
        targetingTimer += Time.deltaTime;
        if (targetingTimer >= targetingTime)
        {
            timerTillDashStart = 0f;
            dashing = true;
            targetingTimer = 0f;
        }

        Vector2 dir = target.position - transform.position;
        float currentAng = transform.rotation.eulerAngles.z;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(currentAng, angle, targetingLerp));
        currentAng = transform.rotation.eulerAngles.z;
        Vector2 targetDir = (new Vector2(Mathf.Cos(currentAng * Mathf.Deg2Rad), Mathf.Sin(currentAng * Mathf.Deg2Rad)));
        targettedPoint = (Vector2) transform.position +
                         (targetDir *
                          dir.magnitude);
        targetLRPoint = (Vector2) transform.position + targetDir * 50f;
    }

    void Dashing()
    {
        targetingLine.startColor = attackingColorLine;
        targetingLine.endColor = attackingColorLine;
        if (timerTillDashStart < timeTillDashStart)
        {
            timerTillDashStart += Time.deltaTime;
            
        }
        else
        {
            if (Vector2.Distance(targettedPoint, transform.position) > distanceToVelZero)
            {
                Vector2 dir = (targettedPoint - (Vector2)transform.position).normalized;
                rb.velocity = dir * dashVelocity;
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, velocityLerpToZero);
                if (rb.velocity.magnitude < threshholdToVelZero)
                {
                    rb.velocity = Vector2.zero;
                    dashing = false;
                }
            }
        }
    }
}

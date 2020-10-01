using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderScript : MonoBehaviour
{
    public float spriteAngleOffset;

    public Transform target;

    public float angleLerp;

    private Rigidbody2D rb;

    public float force;

    public List<Leg> legs = new List<Leg>();
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        AngleStuff();
        MovementStuff();
        DoLegStuff();
    }

    void AngleStuff()
    {
        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - spriteAngleOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, angleLerp));
    }

    void MovementStuff()
    {
        float angle = (transform.rotation.eulerAngles.z + spriteAngleOffset) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        rb.AddForce(dir * force);
    }

    void DoLegStuff()
    {
        for (var i = 0; i < legs.Count; i++)
        {
            if (!legs[i].moving && !legs[legs[i].notSisterLeg].moving)
            {
                if (Vector2.Distance(legs[i].currentPositionTarget.position, legs[i].realTarget.position) >=
                    legs[i].distanceToMove)
                {
                    legs[i].moving = true;
                    StartCoroutine(MoveLeg(legs[i].moveTime, legs[i]));
                    for (var j = 0; j < legs[i].sisterLegs.Length; j++)
                    {
                        legs[legs[i].sisterLegs[j]].moving = true;
                        StartCoroutine(MoveLeg(legs[legs[i].sisterLegs[j]].moveTime, legs[legs[i].sisterLegs[j]]));
                    }
                }
            }
            
        }
    }

    IEnumerator MoveLeg(float duration, Leg legToMove)
    {
        float elapsed = 0.0f;
        Vector3 originalPos = legToMove.currentPositionTarget.position;
        while (elapsed < duration)
        {
            float lerpValue = elapsed / duration;
            float realLerpValue = legToMove.moveCurve.Evaluate(lerpValue);
            legToMove.currentPositionTarget.position = Vector3.Lerp(originalPos, legToMove.realTarget.position, realLerpValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        legToMove.moving = false;
    }
}

[System.Serializable]
public class Leg
{
    public Transform currentPositionTarget;
    public Transform realTarget;
    public float distanceToMove;
    public AnimationCurve moveCurve;
    public float moveTime;
    public bool moving;
    public int[] sisterLegs;
    public int notSisterLeg;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class handBossScript : MonoBehaviour
{
    public Transform rightHandTransform;

    public Transform leftHandTransform;

    public List<Formation> formations;

    private List<handScript> rightHands = new List<handScript>();
    private List<handScript> leftHands = new List<handScript>();

    public Transform spriteHolder;

    public Transform sprite;

    private Rigidbody2D spriteHolderRB;

    public float stage1SpriteHolderLerp;
    public float stage1SpriteHolderMoveForce;

    public bool bossFightStarted;
    public handScript originalHand;
    public handScript originalHand2;

    private SpriteRenderer spriteSr;

    private float spriteSROpacity;
    float opacitySpriteIncreaseTimer;

    private bool isSecondStage;

    private bool secondStageJustStarted;

    public Transform offscreenTargetSecondStageStarted;

    public AnimationCurve secondStageStartMoveCurve;

    public float secondStageStartMoveTime;

    public Transform target;
    public float secondStageSpriteHolderLerp;

    public float secondStageMoveForce;

    private healthScript hS;

    private bool isAttacking;

    private float timeBetweenAttacksTimer;
    public float timeBetweenAttacks;

    public GameObject bulletReg;
    public GameObject bulletSplitter;
    public float secondStageStopMovingTowardDist;

    public AttackSpinningData attackSpinningData;

    private Material spriteMat;
    private float spriteMatAlpha;
    public float spriteMatAlphaChangeRate;

    private cameraShake camerShake;

    private int attack;
    // Start is called before the first frame update
    void Start()
    {
        camerShake = Camera.main.GetComponent<cameraShake>();
        hS = sprite.GetComponent<healthScript>();
        spriteSr = sprite.GetComponent<SpriteRenderer>();
        spriteMat = spriteSr.material;
        spriteHolderRB = spriteHolder.GetComponent<Rigidbody2D>();
        for (var i = 0; i < rightHandTransform.childCount; i++)
        {
            if (rightHandTransform.GetChild(i).GetComponent<handScript>().isActiveAndEnabled)
            {
                rightHands.Add(rightHandTransform.GetChild(i).GetComponent<handScript>());
            }
            
        }

        for (var i = 0; i < leftHandTransform.childCount; i++)
        {
            if (leftHandTransform.GetChild(i).GetComponent<handScript>().isActiveAndEnabled)
            {
                leftHands.Add(leftHandTransform.GetChild(i).GetComponent<handScript>());
            }
            
        }
        CreateFormation(formations[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossFightStarted)
        {
            bossFightStarted = true;
            for (var i = 0; i < rightHands.Count; i++)
            {
                if (!rightHands[i].Equals(null))
                {
                     rightHands[i].lockDown = false;
                }
               
            }
            for (var i = 0; i < leftHands.Count; i++)
            {
                if (!leftHands[i].Equals(null))
                {
                    leftHands[i].lockDown = false;
                }
               
            }
            CreateFormation(formations[Random.Range(0, formations.Count)]);
        }

        if (!isSecondStage)
        {
            bool enabled = false;
            for (var i = 0; i < rightHands.Count; i++)
            {
                if (rightHands[i].disabled == false)
                {
                    enabled = true;
                    break;
                }   
            }

            if(!enabled){
                for (var i = 0; i < leftHands.Count; i++)
                {
                    if (leftHands[i].disabled == false)
                    {
                        enabled = true;
                        break;
                    }
                }
            }

            if (!enabled)
            {
                for (var i = 0; i < rightHands.Count; i++)
                {
                    rightHands[i].hS.actuallyKill = true;
                }
                for (var i = 0; i < leftHands.Count; i++)
                {
                    leftHands[i].hS.actuallyKill = true;
                }
            }
            
            bool canPunch = true;
            int handsActive = 0;
            for (var i = 0; i < rightHands.Count; i++)
            {
                if (!rightHands[i].Equals(null))
                {
                    handsActive += 1;
                    if (rightHands[i].isActiveAndEnabled && !rightHands[i].lockDown)
                    {
                        if (!rightHands[i].readyToPunch)
                        {
                            canPunch = false;
                            break;
                        }
                    }
                }
                
                
            }

            if (canPunch)
            {
                for (var i = 0; i < leftHands.Count; i++)
                {
                    if (!leftHands[i].Equals(null))
                    {
                        handsActive += 1;
                        if (leftHands[i].isActiveAndEnabled && !leftHands[i].lockDown)
                        {
                            if (!leftHands[i].readyToPunch)
                            {
                                canPunch = false;
                                break;
                            }
                        }
                    }

                }
            }

            if (handsActive == 0)
            {
                isSecondStage = true;
                StartCoroutine(StartSecondStage(offscreenTargetSecondStageStarted, secondStageStartMoveTime,
                    secondStageStartMoveCurve));
                secondStageJustStarted = true;
            }
            bool canMove = true;
            for (var i = 0; i < rightHands.Count; i++)
            {
                if (!rightHands[i].Equals(null))
                {
                    if (rightHands[i].isActiveAndEnabled && !rightHands[i].lockDown)
                    {
                        if (!rightHands[i].punched)
                        {
                            canMove = false;
                            break;
                        }
                    }
                }
              

            }

            if (canMove)
            {
                for (var i = 0; i < leftHands.Count; i++)
                {
                    if (!leftHands[i].Equals(null))
                    {
                        if (leftHands[i].isActiveAndEnabled && !leftHands[i].lockDown)
                        {
                            if (!leftHands[i].punched)
                            {
                                canMove = false;
                                break;
                            }
                        }
                    }
                    
                   
                }
            }

            if (canMove)
            {
                CreateFormation(formations[Random.Range(0, formations.Count)]);
            }

            if (canPunch)
            {
                for (var i = 0; i < leftHands.Count; i++)
                {
                    if (!leftHands[i].Equals(null))
                    {
                        if (leftHands[i].isActiveAndEnabled)
                        {
                            leftHands[i].Punch();
                        }
                        
                    }
                    
                }
                for (var i = 0; i < rightHands.Count; i++)
                {
                    if (!rightHands[i].Equals(null))
                    {
                        if (rightHands[i].isActiveAndEnabled)
                        {
                            rightHands[i].Punch();
                        }
                    }
                    
                }
            }
        }
        

        if (bossFightStarted)
        {
            opacitySpriteIncreaseTimer += Time.deltaTime;
            spriteSROpacity = Mathf.Lerp(0f, 1f, opacitySpriteIncreaseTimer);
            spriteMat.SetColor("_Color", new Color(spriteSr.color.r, spriteSr.color.g, spriteSr.color.g, spriteSROpacity));
        }
        CalculateAngleSpriteHolder();
        DoMovementSpriteHolder();
        if (!isSecondStage)
        {
            sprite.rotation = Quaternion.identity;
        }
        else if(isSecondStage && !secondStageJustStarted)
        {
            DoAttacksSecondStage();
        }
        

    }

    private void LateUpdate()
    {
       
    }

    private void CalculateAngleSpriteHolder()
    {
        if (!isSecondStage)
        {
            Vector2 dir = (Vector2.zero - (Vector2)spriteHolder.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            spriteHolder.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(spriteHolder.rotation.eulerAngles.z, angle, stage1SpriteHolderLerp * Time.deltaTime));
        }else if (isSecondStage && !secondStageJustStarted)
        {
            /*Vector2 dir = ((Vector2)target.position - (Vector2)spriteHolder.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            spriteHolder.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(spriteHolder.rotation.eulerAngles.z, angle, secondStageSpriteHolderLerp * Time.deltaTime));*/
        }
       
    }

    void DoMovementSpriteHolder()
    {
        if (!isSecondStage)
        {
            spriteHolderRB.AddRelativeForce(new Vector2(stage1SpriteHolderMoveForce * Time.deltaTime, 0f));
        }
        else
        {
            if (Vector2.Distance(sprite.position, target.position) > secondStageStopMovingTowardDist && !isAttacking)
            {
                Vector2 dir = ((Vector2)target.position - (Vector2)spriteHolder.position).normalized;
                spriteHolderRB.AddForce(dir * (secondStageMoveForce * Time.deltaTime));   
            }
           
        }
        
    }

    void CreateFormation(Formation formation)
    {
        List<handScript> rightHandsToGiveTarget = new List<handScript>();
        List<handScript> leftHandsToGiveTarget = new List<handScript>();
        List<Transform> rightHandTargetsToGive = new List<Transform>();
        List<Transform> leftHandTargetsToGive = new List<Transform>();
        for (var i = 0; i < rightHands.Count; i++)
        {
            if (!rightHands[i].Equals(null))
            {
                if (bossFightStarted)
                {
                    rightHandsToGiveTarget.Add(rightHands[i]);
                }
                else
                {
                    if (!rightHands[i].lockDown)
                    {
                        rightHandsToGiveTarget.Add(rightHands[i]);
                    }
                }
                
            }
            
           
        }
        for (var i = 0; i < leftHands.Count; i++)
        {
            if (!leftHands[i].Equals(null))
            {
                if (bossFightStarted)
                {
                     leftHandsToGiveTarget.Add(leftHands[i]);
                }else
                {
                    if (!leftHands[i].lockDown)
                    {
                        leftHandsToGiveTarget.Add(leftHands[i]);
                    }
                }

               
            }
            
            
            
        }
        for (var i = 0; i < formation.rightHands.Count; i++)
        {
            rightHandTargetsToGive.Add(formation.rightHands[i]);
        }
        for (var i = 0; i < formation.leftHands.Count; i++)
        {
            leftHandTargetsToGive.Add(formation.leftHands[i]);
        }

        for (var i = rightHandsToGiveTarget.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, rightHandTargetsToGive.Count);
            Transform target = rightHandTargetsToGive[index];
            rightHandTargetsToGive.Remove(target);
            rightHandsToGiveTarget[i].target = target;
            rightHandsToGiveTarget[i].goToTarget = true;
            rightHandsToGiveTarget[i].punched = false;
            rightHandsToGiveTarget.Remove(rightHandsToGiveTarget[i]);
        }
        for (var i = leftHandsToGiveTarget.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, leftHandsToGiveTarget.Count);
            Transform target = leftHandTargetsToGive[index];
            leftHandTargetsToGive.Remove(target);
            leftHandsToGiveTarget[i].target = target;
            leftHandsToGiveTarget[i].goToTarget = true;
            leftHandsToGiveTarget[i].punched = false;
            leftHandsToGiveTarget.Remove(leftHandsToGiveTarget[i]);
        }
    }

    void DoAttacksSecondStage()
    {
        if (!isAttacking)
        {
            if (spriteMatAlpha >= 1)
            {
                
            }
            else
            {
                spriteMatAlpha += spriteMatAlphaChangeRate * Time.deltaTime;
            }
            timeBetweenAttacksTimer += Time.deltaTime;
            if (timeBetweenAttacksTimer >= timeBetweenAttacks)
            {
                attack += 1;
                if (attack > 0)
                {
                    attack = 0;
                }
                isAttacking = true;
                timeBetweenAttacksTimer = 0f;
                if (attack == 0)
                {
                    StartCoroutine(AttackSpinning(bulletReg, attackSpinningData.rotSpeed,
                        attackSpinningData.rotAcceleration, attackSpinningData.bulletsReg,
                        attackSpinningData.bulletCountEnd, attackSpinningData.bulletForce, attackSpinningData.duration,
                        attackSpinningData.attacksPerSecond));
                }
                else
                {
                    camerShake.shakes.Add(new Shake(0.3f, 0.2f));
                    StartCoroutine(AttackSplitter(bulletSplitter, attackSpinningData.rotSpeed,
                        attackSpinningData.rotAcceleration, attackSpinningData.bulletForce,
                        attackSpinningData.duration / 1.5f, (int)(attackSpinningData.bulletCountEnd / 2f), bulletReg, 12));
                }
                
            }
        }
        else
        {
            if (spriteMatAlpha <= 0)
            {
                
            }
            else
            {
                spriteMatAlpha -= spriteMatAlphaChangeRate * Time.deltaTime;
            } 
        }
        spriteMat.SetFloat("_Alpha", spriteMatAlpha);
    }

    void ShootRadial(GameObject bulletPref, int bulletCount, float bulletForce, float distFromCenter)
    {
        float initAngle = transform.rotation.eulerAngles.z;
        float increment = 360f / bulletCount;
        for (var i = 0; i < bulletCount; i++)
        {
            float currAngle = initAngle + (increment * i);
            currAngle *= Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            Vector2 instantiationPos = (Vector2)sprite.transform.position + (dir * distFromCenter);
            GameObject bulletObj = GameObject.Instantiate(bulletPref, instantiationPos, Quaternion.Euler(0f, 0f, currAngle * Mathf.Rad2Deg));
            bulletObj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(bulletForce,0f));
            bulletObj.GetComponent<bulletScript>().shooter = hS;
            // Destroy(GameObject.Instantiate(shootBulletParticles, shootParticlesTransform.position, transform.rotation),3f);
           // camerShake.shakes.Add(new Shake(0.1f, 0.1f));
        }
       
    }

    IEnumerator AttackSpinning(GameObject bulletPref, float rotationSpeed, float rotationAcceleration, int bulletCountReg, int bulletCountEnd, float bulletForce, float duration, float attacksPerSecond)
    {
        Collider2D[] collider2Ds = sprite.GetComponents<Collider2D>();
        for (var i = 0; i < collider2Ds.Length; i++)
        {
            collider2Ds[i].enabled = true;
        }
        float elapsed = 0.0f;
        float shootTime = 0.0f;
        float currentAngularVel = 0.0f;
        while (elapsed < duration)
        {
            if (currentAngularVel < rotationSpeed)
            {
                currentAngularVel += rotationAcceleration;
            }
            sprite.Rotate(0f, 0f, currentAngularVel * Time.deltaTime);
            if (shootTime > 1 / attacksPerSecond)
            {
                ShootRadial(bulletPref, bulletCountReg, bulletForce, 1f);
                shootTime = 0f;
            }
            elapsed += Time.deltaTime;
            shootTime += Time.deltaTime;
            yield return null;
        }
        camerShake.shakes.Add(new Shake(0.4f, 0.25f));
        ShootRadial(bulletPref, bulletCountEnd, bulletForce, 1f);
        isAttacking = false;
        collider2Ds = sprite.GetComponents<Collider2D>();
        for (var i = 0; i < collider2Ds.Length; i++)
        {
            collider2Ds[i].enabled = false;
        }
    }

    IEnumerator AttackSplitter(GameObject bulletPref, float rotationSpeed, float rotationAcceleration, float bulletForce, float duration, int bulletCountEnd, GameObject bulletPrefEnd, int bulletCount)
    {
        Collider2D[] collider2Ds = sprite.GetComponents<Collider2D>();
        for (var i = 0; i < collider2Ds.Length; i++)
        {
            collider2Ds[i].enabled = true;
        }
        float elapsed = 0.0f;
        float currentAngularVel = 0.0f;
        ShootRadial(bulletPref, bulletCount, bulletForce, 1f);
        while (elapsed < duration)
        {
            if (currentAngularVel < rotationSpeed)
            {
                currentAngularVel += rotationAcceleration;
            }
            sprite.Rotate(0f, 0f, currentAngularVel * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        camerShake.shakes.Add(new Shake(0.4f, 0.25f));
        ShootRadial(bulletPrefEnd, bulletCountEnd, bulletForce, 1f);
        isAttacking = false;
        collider2Ds = sprite.GetComponents<Collider2D>();
        for (var i = 0; i < collider2Ds.Length; i++)
        {
            collider2Ds[i].enabled = false;
        }
    }

    IEnumerator StartSecondStage(Transform offscreenTarget, float timeToFirstTarg, AnimationCurve curveToTarg)
    {
        spriteHolderRB.simulated = false;
        float elapsed = 0.0f;
        Vector2 startPos = spriteHolder.position;
        while (elapsed < timeToFirstTarg)
        {
            float lerpVal = curveToTarg.Evaluate(elapsed / timeToFirstTarg);
            spriteHolder.position = Vector2.Lerp(startPos, offscreenTarget.position, lerpVal);
            elapsed += Time.deltaTime;
            yield return null;
        }
        sprite.localPosition = Vector3.zero;
        spriteSr.color = Color.white;
        spriteHolderRB.simulated = true;
        spriteSr.sortingOrder = 2;
        /*Collider2D[] collider2Ds = sprite.GetComponents<Collider2D>();
        for (var i = 0; i < collider2Ds.Length; i++)
        {
            collider2Ds[i].enabled = true;
        }*/

        spriteHolder.rotation = Quaternion.identity;
        sprite.rotation = Quaternion.identity;
        secondStageJustStarted = false;
    }
}

[System.Serializable]
public class Formation
{
    public List<Transform> rightHands;
    public List<Transform> leftHands;
}

[System.Serializable]
public class AttackSpinningData
{
    public int bulletsReg;
    public float rotSpeed;
    public float rotAcceleration;
    public int bulletCountEnd;
    public float bulletForce;
    public float duration;
    public float attacksPerSecond;
}

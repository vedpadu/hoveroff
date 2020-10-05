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
    // Start is called before the first frame update
    void Start()
    {
        spriteSr = sprite.GetComponent<SpriteRenderer>();
        spriteHolderRB = spriteHolder.GetComponent<Rigidbody2D>();
        for (var i = 0; i < rightHandTransform.childCount; i++)
        {
            rightHands.Add(rightHandTransform.GetChild(i).GetComponent<handScript>());
        }

        for (var i = 0; i < leftHandTransform.childCount; i++)
        {
            leftHands.Add(leftHandTransform.GetChild(i).GetComponent<handScript>());
        }
        CreateFormation(formations[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (originalHand.Equals(null) && originalHand2.Equals(null) && !bossFightStarted)
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
        bool canPunch = true;
        for (var i = 0; i < rightHands.Count; i++)
        {
            if (!rightHands[i].Equals(null))
            {
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

        if (bossFightStarted)
        {
            opacitySpriteIncreaseTimer += Time.deltaTime;
            spriteSROpacity = Mathf.Lerp(0f, 1f, opacitySpriteIncreaseTimer);
            spriteSr.color = new Color(spriteSr.color.r, spriteSr.color.g, spriteSr.color.g, spriteSROpacity);
        }
            CalculateAngleSpriteHolder();
                DoMovementSpriteHolder();


    }

    private void LateUpdate()
    {
        sprite.rotation = Quaternion.identity;
    }

    private void CalculateAngleSpriteHolder()
    {
        Vector2 dir = (Vector2.zero - (Vector2)spriteHolder.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        spriteHolder.rotation = Quaternion.Euler(0f,0f,Mathf.LerpAngle(spriteHolder.rotation.eulerAngles.z, angle, stage1SpriteHolderLerp * Time.deltaTime));
    }

    void DoMovementSpriteHolder()
    {
        spriteHolderRB.AddRelativeForce(new Vector2(stage1SpriteHolderMoveForce * Time.deltaTime, 0f));
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
}

[System.Serializable]
public class Formation
{
    public List<Transform> rightHands;
    public List<Transform> leftHands;
}

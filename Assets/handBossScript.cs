using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handBossScript : MonoBehaviour
{
    public Transform rightHandTransform;

    public Transform leftHandTransform;

    public List<Formation> formations;

    private List<handScript> rightHands = new List<handScript>();
    private List<handScript> leftHands = new List<handScript>();
    
    // Start is called before the first frame update
    void Start()
    {
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
        bool canPunch = true;
        for (var i = 0; i < rightHands.Count; i++)
        {
            if (!rightHands[i].readyToPunch)
            {
                canPunch = false;
                break;
            }
        }

        if (canPunch)
        {
            for (var i = 0; i < leftHands.Count; i++)
            {
                if (!leftHands[i].readyToPunch)
                {
                    canPunch = false;
                    break;
                }
            }
        }

        if (canPunch)
        {
            for (var i = 0; i < leftHands.Count; i++)
            {
                leftHands[i].Punch();
            }
            for (var i = 0; i < rightHands.Count; i++)
            {
                rightHands[i].Punch();
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
            rightHandsToGiveTarget.Add(rightHands[i]);
        }
        for (var i = 0; i < leftHands.Count; i++)
        {
            leftHandsToGiveTarget.Add(leftHands[i]);
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
            rightHandsToGiveTarget.Remove(rightHandsToGiveTarget[i]);
        }
        for (var i = leftHandsToGiveTarget.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, leftHandsToGiveTarget.Count);
            Transform target = leftHandTargetsToGive[index];
            leftHandTargetsToGive.Remove(target);
            leftHandsToGiveTarget[i].target = target;
            leftHandsToGiveTarget[i].goToTarget = true;
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

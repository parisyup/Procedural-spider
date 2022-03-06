using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyController : MonoBehaviour
{
    public GameObject spider;
    public GameObject[] legTips; //to get the leg positions (the tip of the leg that i used in my old video)
    public GameObject[] legTargets;
    public GameObject[] legCubes;

    public bool enableBodyRotation = false;
    public bool enableMovementRotation = false;
    Vector3 velocity;
    Vector3 lastVelocity = Vector3.one;
    Vector3 lastSpiderPosition;

    Vector3[] legPositions;
    Vector3[] legOriginalPositions;
    List<int> nextIndexToMove = new List<int>();
    public float legMoveSpeed = 7f;
    public float moveDistance = 0.7f;
    public float stepHeight = .15f;
    public Vector3 lastBodyUp;
    public float LegSmoothness = 8; //to smooth out the movement of the body
    public float BodySmoothness = 8; //to smooth out the movement of the body

    public float multiplier = 4;

    public float resetTimer = 0.5f;

    float setup = 0.2f;

    void Start()
    {
        lastBodyUp = transform.up; //setting the lastbody up which is basically just the up side of the body i have attached a photo to try and explain it

        legPositions = new Vector3[legTargets.Length];
        legOriginalPositions = new Vector3[legTargets.Length];
        //isMoving = new bool[legTargets.Length];

        for (int i = 0; i < legTargets.Length; i++)
        {
            legPositions[i] = legTargets[i].transform.position;
            legOriginalPositions[i] = legTargets[i].transform.position;
        }

        lastSpiderPosition = spider.transform.position;
        rotateBody();
    }


    void Update()
    {
        

        velocity = spider.transform.position - lastSpiderPosition;
        velocity = (velocity + BodySmoothness * lastVelocity) / (BodySmoothness + 1f);

        
        if (setup > 0) {setup -= Time.deltaTime; return; }
        
        moveLegs();
        rotateBody();

        lastSpiderPosition = spider.transform.position;

        lastVelocity = velocity;

    }

    void moveLegs()
    {
        if (!enableMovementRotation) return;
        for (int i = 0; i < legTargets.Length; i++)
        {
            if (Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= moveDistance)
            {
                if (!nextIndexToMove.Contains(i)) nextIndexToMove.Add(i);
            }
            else if (!nextIndexToMove.Contains(i))
            {
                legTargets[i].transform.position = legOriginalPositions[i];
            }

        }

        if (nextIndexToMove.Count == 0) return;
        Vector3 targetPosition = legCubes[nextIndexToMove[0]].transform.position + Mathf.Clamp(velocity.magnitude * multiplier, 0.0f, 1.5f) * (legCubes[nextIndexToMove[0]].transform.position - legTargets[nextIndexToMove[0]].transform.position) + velocity * multiplier;
        StartCoroutine(step(nextIndexToMove[0], targetPosition));
        if (nextIndexToMove.Count >= legPositions.Length)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0) nextIndexToMove.Clear();
        }
        else resetTimer = 0.5f;
    }

    IEnumerator step(int index, Vector3 moveTo)
    {
        Vector3 startPos = legOriginalPositions[index];

        for (int i = 1; i <= LegSmoothness; ++i)
        {
            legTargets[index].transform.position = Vector3.Lerp(startPos, moveTo + new Vector3(0, Mathf.Sin(i / (float)(LegSmoothness + 1f) * Mathf.PI) * stepHeight, 0), (i / LegSmoothness + 1f) * legMoveSpeed);
            yield return new WaitForFixedUpdate();
        }


        legTargets[index].transform.position = moveTo;
        legOriginalPositions[index] = moveTo;

        if (nextIndexToMove.Count != 0)
            nextIndexToMove.RemoveAt(0);


    }

    void rotateBody()
    {
        if (!enableBodyRotation) return;
        Vector3 v1 = legTips[0].transform.position - legTips[1].transform.position;
        Vector3 v2 = legTips[2].transform.position - legTips[3].transform.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(BodySmoothness + 1));
        transform.up = up;
        lastBodyUp = up;
    }
}

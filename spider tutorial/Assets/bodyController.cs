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
    public float moveStoppingDistance = 0.4f;
    public Vector3 lastBodyUp;
    public float smoothness = 8; //to smooth out the movement of the body

    public float multiplier = 4;

    
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
    }


    void Update()
    {
        rotateBody();   
        velocity = spider.transform.position - lastSpiderPosition;
        velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);
        //Debug.Log(velocity);

        moveLegs(); 
        
        lastSpiderPosition = spider.transform.position;
            
        lastVelocity = velocity;
        
    }

    void moveLegs()
    {
        if (!enableMovementRotation) return;
        for(int i = 0; i < legTargets.Length; i++)
        {
            if(Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= moveDistance)
            {
                nextIndexToMove.Add(i);
            }
            else
            {
                legTargets[i].transform.position = legOriginalPositions[i];
            }
            
        }

        if (nextIndexToMove.Count == 0) return;
        Vector3 targetPosition = legCubes[nextIndexToMove[0]].transform.position + Mathf.Clamp(velocity.magnitude * multiplier, 0.0f, 1.5f) * (legCubes[nextIndexToMove[0]].transform.position - legTargets[nextIndexToMove[0]].transform.position) + velocity * multiplier;
        StartCoroutine(step(nextIndexToMove[0], targetPosition));
    }

    IEnumerator step(int index, Vector3 moveTo)
    {
        Vector3 startPos = legOriginalPositions[index];

        legTargets[index].transform.position = Vector3.Lerp(startPos, moveTo + new Vector3(0,0.5f,0), legMoveSpeed * Time.deltaTime);
        //legTargets[index].transform.position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
        yield return new WaitForFixedUpdate();

        if (Vector3.Distance(legTargets[index].transform.position, moveTo) <= moveStoppingDistance)
        {
            legTargets[index].transform.position = moveTo - new Vector3(0, 0.5f, 0);
            legOriginalPositions[index] = moveTo - new Vector3(0, 0.5f, 0);

            if (nextIndexToMove.Count != 0)
                nextIndexToMove.RemoveAt(0);
        }

    }

    void rotateBody()
    {
        if (!enableBodyRotation) return;
        Vector3 v1 = legTips[0].transform.position - legTips[1].transform.position;
        Vector3 v2 = legTips[2].transform.position - legTips[3].transform.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(smoothness + 1));
        transform.up = up;
        lastBodyUp = up;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killonstart : MonoBehaviour
{

    float timeToDeath = 0.1f;
    void Update()
    {
	timeToDeath -= Time.deltaTime;
    	if(timeToDeath > 0) return;
	Destroy(gameObject);   
    }
}

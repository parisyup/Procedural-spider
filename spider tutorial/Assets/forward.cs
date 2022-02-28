using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forward : MonoBehaviour
{
    public Transform body;
    void Update()
    {
        Debug.Log(Mathf.Rad2Deg * body.rotation.y );
        transform.rotation = new Quaternion(transform.rotation.x, body.rotation.y, transform.rotation.z, transform.rotation.w);
    }
}

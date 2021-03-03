using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update() 
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        transform.position = new Vector3(transform.position.x, transform.localScale.y/2 * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.x), transform.position.z);
    }
}

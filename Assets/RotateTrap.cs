using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTrap : MonoBehaviour
{
    public float DegreesPerSecond = 11f;
    // Start is called before the first frame update
    void Start()
    {
        rotationVector = transform.rotation.eulerAngles;
    }

    private Vector3 rotationVector;                
    // Update is called once per frame
    void Update()
    { 
        // Spin the object around at the given degrees/second.
        transform.RotateAround(transform.position, Vector3.up, DegreesPerSecond * Time.deltaTime);
    }
}

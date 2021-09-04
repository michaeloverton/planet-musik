using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGrow : MonoBehaviour
{
    public int growthExponent = 2;
    public float minX = 0.25f;
    public float minY = 0.25f;
    public float minZ = 0.25f;
    public float maxX = 3f;
    public float maxY = 3f;
    public float maxZ = 3f;

    public float xSpeed = 3f;
    public float ySpeed = 3f;
    public float zSpeed = 3f;

    private bool grow = false;
    public Transform growingObject;
    float distance;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter(Collider other) {
        grow = true;
    }

    void OnTriggerExit(Collider other) {
        grow = false;
    }

    void OnTriggerStay(Collider other) {
        distance = Vector3.Magnitude(transform.position - other.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(grow) {
            if(growingObject.localScale.x < maxX) {
                growingObject.localScale += Vector3.right * (1/Mathf.Pow(distance, growthExponent)) * xSpeed * Time.deltaTime;
            }
            if(growingObject.localScale.y < maxY) {
                growingObject.localScale += Vector3.up * (1/Mathf.Pow(distance, growthExponent)) * ySpeed * Time.deltaTime;
            }
            if(growingObject.localScale.z < maxZ) {
                growingObject.localScale += Vector3.forward * (1/Mathf.Pow(distance, growthExponent)) * zSpeed * Time.deltaTime;
            }
        } else {
            if(growingObject.localScale.x > minX) {
                growingObject.localScale -= Vector3.right * xSpeed * Time.deltaTime;
            }
            if(growingObject.localScale.y > minY) {
                growingObject.localScale -= Vector3.up * ySpeed * Time.deltaTime;
            }
            if(growingObject.localScale.x > minZ) {
                growingObject.localScale -= Vector3.forward * zSpeed * Time.deltaTime;
            }
        }
    }

}

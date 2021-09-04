using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGrow : MonoBehaviour
{
    public float minX;
    public float minY;
    public float minZ;
    public float maxX;
    public float maxY;
    public float maxZ;

    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

    private bool grow = false;
    public Transform growingObject;

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

    // Update is called once per frame
    void Update()
    {
        if(grow) {
            if(growingObject.localScale.x < maxX) {
                growingObject.localScale += Vector3.right * xSpeed * Time.deltaTime;
            }
            if(growingObject.localScale.y < maxY) {
                growingObject.localScale += Vector3.up * ySpeed * Time.deltaTime;
            }
            if(growingObject.localScale.z < maxZ) {
                growingObject.localScale += Vector3.forward * zSpeed * Time.deltaTime;
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

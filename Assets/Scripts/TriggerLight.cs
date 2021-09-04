using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLight : MonoBehaviour
{
    public Light trigLight;
    public float maxIntensity = 3.5f;
    public float minIntensity = 0f;
    bool increaseIntensity = false;
    float originalIntensity;
    float increaseSpeed = 2.0f;
    float distance;

    void Start()
    {
        originalIntensity = trigLight.intensity;
    }

    void OnTriggerEnter(Collider other) {
        increaseIntensity = true;
    }

    void OnTriggerExit(Collider other) {
        increaseIntensity = false;
    }

    void OnTriggerStay(Collider other) {
        distance = Vector3.Magnitude(transform.position - other.transform.position);
    }

    void FixedUpdate() {
        if(increaseIntensity) {
            if(trigLight.intensity < maxIntensity) {
                trigLight.intensity += (1/Mathf.Pow(distance, 1)) * increaseSpeed * Time.fixedDeltaTime;
            }
        } 
        else {
            if(trigLight.intensity > minIntensity) {
                trigLight.intensity -= increaseSpeed * Time.fixedDeltaTime;
            }
        }
    }
}

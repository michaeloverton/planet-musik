using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLight : MonoBehaviour
{
    Light trigLight;
    SphereCollider trigger;
    bool increaseIntensity = false;
    float originalIntensity;
    float increaseSpeed = 2.0f;
    float distance;

    void Start()
    {
        trigLight = GetComponent<Light>();
        if(!trigLight) {
            throw new System.Exception("couldn't find light component to trigger");
        }

        trigger = GetComponent<SphereCollider>();
        if(!trigger) {
            throw new System.Exception("could not find trigger collider");
        }

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
            // Debug.Log("increasing intensity");
            trigLight.intensity += Mathf.Clamp((1/Mathf.Pow(distance, 1)) * increaseSpeed * Time.fixedDeltaTime, 0, 6);
            trigLight.range += Mathf.Clamp((1/Mathf.Pow(distance, 1)) * increaseSpeed * Time.fixedDeltaTime, 7, 30);
        } 
        else {
            trigLight.intensity -= Mathf.Clamp((1/Mathf.Pow(distance, 1)) * increaseSpeed * Time.fixedDeltaTime, 0, 6);
            trigLight.range -= Mathf.Clamp((1/Mathf.Pow(distance, 1)) * increaseSpeed * Time.fixedDeltaTime, 7, 30);
        }

        trigLight.intensity = Mathf.Clamp(trigLight.intensity, 0, 6);
        trigLight.range = Mathf.Clamp(trigLight.range, 7, 30);
    }
}

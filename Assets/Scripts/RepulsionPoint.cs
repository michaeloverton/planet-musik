using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsionPoint : MonoBehaviour
{
    SphereCollider repulsionTrigger;
    Rigidbody body;
    Vector3 originalPosition;
    Vector3 forceDirection;
    float forceIntensity = 10.0f;
    float forceAmount;
    public float reboundSpeed = 5.0f;
    bool applyForce = false;

    // Start is called before the first frame update
    void Start()
    {
        repulsionTrigger = GetComponent<SphereCollider>();
        if(!repulsionTrigger) {
            throw new System.Exception("could not find repulsion trigger collider");
        }

        body = GetComponentInParent<Rigidbody>();
        if(!body) {
            throw new System.Exception("could not find rigidbody in repulsion point parent");
        }

        originalPosition = transform.position;
    }

    void OnTriggerEnter(Collider other) {
        if(other.GetComponent<RepellerPoint>()) {
            // Debug.Log("repeller point enter.");
            applyForce = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.GetComponent<RepellerPoint>()) {
            applyForce = false;
        }
    }

    void OnTriggerStay(Collider other) {
        if(other.GetComponent<RepellerPoint>()) {
            float distanceBetweenColliders = Vector3.Magnitude(transform.position - other.transform.position);
            forceDirection = (transform.position - other.transform.position) / Vector3.Magnitude(transform.position - other.transform.position);
            forceAmount = 1/Mathf.Pow(distanceBetweenColliders, 2);
        }
    }

    void FixedUpdate() {
        if(applyForce) {
            body.AddForce(100 * forceAmount * forceDirection * Time.fixedDeltaTime);
        } else {
            // If nothing pushing, rebound back to original position.
            // MoveTowards requires Rigidbody interpolation to be enabled for smooth movement.
            // ONLY DO THIS IF THE DISTANCE IS LARGER THAN SOME TOLERANCE
            // Vector3 moveToPosition = Vector3.MoveTowards(body.position, originalPosition, reboundSpeed * Time.fixedDeltaTime);
            // body.MovePosition(moveToPosition);
            // Vector3 displacementVector = body.position - originalPosition;
            Vector3 displacementVector = originalPosition - body.position;
            body.AddForce(displacementVector * Vector3.Magnitude(displacementVector) * reboundSpeed * Time.fixedDeltaTime);
        }
    }
}

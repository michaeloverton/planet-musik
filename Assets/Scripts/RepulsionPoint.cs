using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsionPoint : MonoBehaviour
{
    SphereCollider repulsionTrigger;
    Rigidbody body;
    Vector3 originalPosition;
    Vector3 forceDirection;
    public float repulsionForceIntensity = 100.0f;
    float forceAmount;
    public float reboundSpeed = .75f;
    bool applyForce = false;
    bool countdownRebound = false;
    public float reboundAfter = 0.25f;
    float currentTimeAfterExit = 0.0f;
    bool canRebound = false;
    public float reboundDetectionRadius = 0.1f;

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

    void Update() {
        if(countdownRebound) {
            currentTimeAfterExit += Time.deltaTime;
            if(currentTimeAfterExit > reboundAfter) {
                
                // Only actually begin to rebound if the activator is far enough away.
                // 8 represents the layer index of the activator.
                int layerMask = 1 << 8; // Layermask is a bitmask: https://answers.unity.com/questions/1177883/overlapsphere-ignoring-all-colliders-when-i-use-th.html
                Collider[] colliders = Physics.OverlapSphere(transform.position, repulsionTrigger.radius + reboundDetectionRadius, layerMask);
                if(colliders.Length == 0) {
                    canRebound = true;
                    countdownRebound = false;
                    currentTimeAfterExit = 0;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        applyForce = true;

        // No rebounding once an activator has entered.
        countdownRebound = false;
        canRebound = false;
    }

    void OnTriggerExit(Collider other) {
        applyForce = false;

        // When we leave the activation trigger, there is a window of time before we can rebound to original position.
        countdownRebound = true;
    }

    void OnTriggerStay(Collider other) {
        float distanceBetweenColliders = Vector3.Magnitude(transform.position - other.transform.position);
        forceDirection = (transform.position - other.transform.position) / Vector3.Magnitude(transform.position - other.transform.position);
        forceAmount = 1/Mathf.Pow(distanceBetweenColliders, 2);
    }

    void FixedUpdate() {
        if(applyForce) {
            body.AddForce(repulsionForceIntensity * forceAmount * forceDirection * Time.fixedDeltaTime);
        } else if(canRebound) {
            // If nothing pushing, rebound back to original position.
            // MoveTowards requires Rigidbody interpolation to be enabled for smooth movement.
            Vector3 moveToPosition = Vector3.MoveTowards(body.position, originalPosition, reboundSpeed * Time.fixedDeltaTime);
            body.MovePosition(moveToPosition);
        }
    }
}

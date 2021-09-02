using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDrag : MonoBehaviour
{
    public float drag = 0.2f;
    public float angularDrag = 0.2f;

    void Start()
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody body in bodies) {
            body.drag = drag;
            body.angularDrag = angularDrag;
        }
    }
}

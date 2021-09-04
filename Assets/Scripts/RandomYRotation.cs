using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomYRotation : MonoBehaviour
{
    void Start()
    {
        transform.Rotate(new Vector3(0, Random.Range(0f, 360f), 0), Space.World);  
    }
}

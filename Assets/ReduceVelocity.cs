using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReduceVelocity : MonoBehaviour
{

    void Update()
    {
        float dot = Vector3.Dot(GetComponent<Rigidbody>().velocity, Vector3.right);

        GetComponent<Rigidbody>().velocity -= Vector3.right * dot * 0.5f;
    }
}

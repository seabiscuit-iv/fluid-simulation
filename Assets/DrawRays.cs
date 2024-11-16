using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRays : MonoBehaviour
{
    Vector3 lastpos;

    // Start is called before the first frame update
    void Start()
    {
        lastpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(lastpos, transform.position, Color.blue, 1f);
        Debug.Log(lastpos != transform.position);

        lastpos = transform.position;
    }
}

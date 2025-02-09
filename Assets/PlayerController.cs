using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0.0f, 0.0f);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TriangleRenderer : MonoBehaviour
{
    public GameObject positionTrackerObj;

    List<Vector3> positions;
    List<int> indicies;

    public float width = 0.01f;

    Mesh mesh;

    private void Start() {
        mesh = new();
        positions = new();
        indicies = new();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        if(positionTrackerObj != null) {
            
            Vector3 pos = positionTrackerObj.transform.position;

            Vector3 first = pos + new Vector3(width, 0f, 0f);
            Vector3 second = pos - new Vector3(width, 0f, 0f);

            if(positions.Count != 0) {
                int f = positions.Count - 2;
                int s = positions.Count - 1;

                indicies.AddRange(new int[] {
                    f, s, positions.Count + 1,
                    s, f, positions.Count + 1,
                    f, positions.Count,  positions.Count + 1,
                    positions.Count, f, positions.Count + 1,
                });
            }

            positions.Add(first);
            positions.Add(second);

            if(positions.Count >= 600 ) {
                Debug.Log("Removing");
                positions.RemoveAt(0);
                positions.RemoveAt(0);

                for(int i = 0; i < 12; i++) {
                    indicies.RemoveAt(0);
                }

                for(int i = 0; i < indicies.Count; i++) {
                    indicies[i] -= 2;
                }
            }   
        } else {
            if(indicies.Count == 0) {
                Destroy(gameObject);
            }

            Debug.Log("Removing Back");

            positions.RemoveAt(0);
            positions.RemoveAt(0);


            for(int i = 0; i < 12; i++) {
                indicies.RemoveAt(0);
                indicies.RemoveAt(0);
            }

            
            // for(int i = 0; i < indicies.Count; i++) {
            //     indicies[i];
            // }
        }

        mesh.vertices = positions.ToArray();
        mesh.triangles = indicies.ToArray();
        mesh.RecalculateNormals();
    }
}

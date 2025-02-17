using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FluidParticleManager : MonoBehaviour
{
    public List<GameObject> waterParticles;

    public Dictionary<Vector3, List<GameObject>> neighbors;

    public float cohesionStrength = 1.0f;
    public float separationStrength = 2.0f;
    public float separationDistance = 0.5f;

    public float viscosity = 0.5f;

    public void ForceAdd(GameObject obj, int max) {
        if (waterParticles.Count >= max) {
            int numToRemove = waterParticles.Count - (max-1);
            waterParticles.GetRange(0, numToRemove).ForEach((x) => Destroy(x));
            waterParticles.RemoveRange(0, numToRemove);
        }
        waterParticles.Add(obj);
    }

    void Awake()
    {
        waterParticles = new();
    }

    void FixedUpdate()
    {
        RecalculateNeighbors();

        int max = 0;
        foreach (List<GameObject> obj in neighbors.Values) {
            max = Math.Max(obj.Count(), max);
        }

        // Debug.Log("Max: " + max);

        CohesionSeparationForces();
        Viscosity();
    }

    
    void RecalculateNeighbors() {
        neighbors = new();

        foreach (GameObject particle in waterParticles) {
            Vector3 posHash = new (Mathf.Floor(particle.transform.position.x * 2.0f), Mathf.Floor(particle.transform.position.y * 2.0f), Mathf.Floor(particle.transform.position.z * 2.0f));
            
            if (!neighbors.ContainsKey(posHash)) {
                neighbors.Add(posHash, new());
            }

            neighbors[posHash].Add(particle);
        }
    }


    void CohesionSeparationForces() {
        foreach (List<GameObject> chunk in neighbors.Values) {
            Vector3 sumPos = new();
            int count = -1;
            foreach (GameObject particle in chunk) {
                sumPos += particle.transform.position;
                count++;
            }
            
            if(count == 0) {
                continue;
            }

            foreach (GameObject particle in chunk) {
                Vector3 particlePos = particle.transform.position;
                Vector3 avgNeighbors = (sumPos - particlePos) / count;

                float dist = Vector3.Distance(avgNeighbors, particlePos);

                if (dist > separationDistance) {
                    // bring point closer to average
                    particle.GetComponent<Rigidbody>().AddForce(cohesionStrength * Vector3.Normalize(avgNeighbors - particlePos));
                } else {
                    // bring point farther to average
                    particle.GetComponent<Rigidbody>().AddForce(separationStrength * -Vector3.Normalize(avgNeighbors - particlePos));
                }
            }
        }
    }

    void Viscosity() {
        foreach (List<GameObject> chunk in neighbors.Values) {
            Vector3 avgVelocity = new();
            int count = 0;
            foreach (GameObject particle in chunk) {
                avgVelocity += particle.GetComponent<Rigidbody>().velocity;
                count++;
            }
            avgVelocity /= count;

            foreach (GameObject particle in chunk) {
                var rb = particle.GetComponent<Rigidbody>();
                Vector3 particleVelocity = rb.velocity;

                //Mix
                rb.velocity = Vector3.Lerp(particleVelocity, avgVelocity, viscosity);
                //Viscocity Blend
                // Vector3 velocityDifference = avgVelocity - rb.velocity;
                // rb.velocity += velocityDifference * viscosity;
            }
        }
    }

}

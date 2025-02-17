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
    public bool viscocityInterpolation = true;

    public float vorticityStrength = 0.4f;

    public float noiseScale = 0.1f;
    public float noiseStrength = 0.5f;

    public float shearStrength = 0.2f;

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
        VorticityConfinement();
        ShearForces();
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

                if (viscocityInterpolation) {
                    //Mix
                    rb.velocity = Vector3.Lerp(particleVelocity, avgVelocity, viscosity);
                } else {
                    Vector3 viscosityForce = (avgVelocity - rb.velocity) * viscosity;
                    rb.AddForce(viscosityForce);
                }
                //Viscocity Blend
                // Vector3 velocityDifference = avgVelocity - rb.velocity;
                // rb.velocity += velocityDifference * viscosity;
            }
        }
    }

    
    void VorticityConfinement() {
        foreach (List<GameObject> chunk in neighbors.Values) {
            foreach (GameObject particle in chunk) {
                Vector3 curl = new();
                Rigidbody rb = particle.GetComponent<Rigidbody>();

                foreach (GameObject other in chunk) {
                    if (particle == other) {
                        continue;
                    }

                    Rigidbody otherRb = other.GetComponent<Rigidbody>();
                    curl += Vector3.Cross(otherRb.velocity - rb.velocity, other.transform.position - particle.transform.position);
                }

                Vector3 confinementForce = curl.normalized * vorticityStrength;
                rb.AddForce(confinementForce);
            }
        }
    }


    // Not that great tbh
    void VelocityNoise() {
        foreach (List<GameObject> chunk in neighbors.Values) {
            foreach (GameObject particle in chunk) {
                Rigidbody rb = particle.GetComponent<Rigidbody>();
                float noise_1 = Mathf.PerlinNoise(particle.transform.position.x * noiseScale + 23.53f, particle.transform.position.y * noiseScale + 21.67f);
                float noise_2 = Mathf.PerlinNoise(particle.transform.position.x * noiseScale + 13.854f, particle.transform.position.y * noiseScale + 11.63f);
                float noise_3 = Mathf.PerlinNoise(particle.transform.position.x * noiseScale + 11.11f, particle.transform.position.y * noiseScale + 234.6532f);
                Vector3 noiseForce = new Vector3(noise_1, noise_2, noise_3).normalized * noiseStrength;
                rb.AddForce(noiseForce);
            }
        }
    }


    void ShearForces() {
        foreach (List<GameObject> chunk in neighbors.Values) {
            foreach (GameObject particle in chunk) {
                Vector3 shearForce = new();
                Rigidbody rb = particle.GetComponent<Rigidbody>();

                int count = 0;
                foreach (GameObject other in chunk) {
                    if (particle == other) {
                        continue;
                    }

                    Rigidbody otherRb = other.GetComponent<Rigidbody>();
                    
                    Vector3 velocityDiff = otherRb.velocity - rb.velocity;
                    Vector3 perpendicularForce = Vector3.Cross(velocityDiff, Vector3.up);
                    shearForce += perpendicularForce;
                    count++;
                }

                if (count != 0) {
                    shearForce /= count;
                    rb.AddForce(shearForce * shearStrength);       
                }
            }
        }
    }
}

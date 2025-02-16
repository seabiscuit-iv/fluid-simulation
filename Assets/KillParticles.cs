using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillParticles : MonoBehaviour
{
    public FluidParticleManager particleManager;

    private void OnCollisionEnter(Collision other) {
        particleManager.waterParticles.Remove(other.gameObject);
        Destroy(other.gameObject);
    }
}

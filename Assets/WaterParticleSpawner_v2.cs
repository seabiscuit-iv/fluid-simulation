
using System.Collections;
using UnityEngine;

public class WaterParticleSpawner_v2 : MonoBehaviour
{
    
    public int xAmt = 4;
    public int yAmt = 2;

    public int maxParticles = 150;

    public GameObject waterParticle;
    public FluidParticleManager fluidParticleManager;

    private void Start() {
        StartCoroutine(spawn());
    }

    int count = 0;

    private IEnumerator spawn() {
        while(true) {
            for(float i = -((float)xAmt/2); i < (float)xAmt / 2; i++) {
                for(float j = -((float)yAmt/2); j < (float)yAmt/2; j++) {

                    float noise = Mathf.PerlinNoise(i / 14 * 234.123478f * ((count + 4) / 2.234f) , j/5);

                    // if (fluidParticleManager.waterParticles.Count <= maxParticles) {
                        GameObject obj = Instantiate(waterParticle, new Vector3(0f, 0f, 0f), Quaternion.identity);

                        obj.transform.position = 
                            transform.position + new Vector3(i/2 + noise * 6f - 3f, j/5 + noise * 5, 0);

                        // fluidParticleManager.waterParticles.Add(obj);
                        fluidParticleManager.ForceAdd(obj, maxParticles);
                    // }
                }
            }

            yield return new WaitForSeconds(1.2f);        
            
            count++;
        }
    }
}

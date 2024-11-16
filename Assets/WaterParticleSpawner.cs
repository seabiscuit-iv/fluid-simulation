
using System.Collections;
using UnityEngine;

public class WaterParticleSpawner : MonoBehaviour
{
    
    public int xAmt = 4;
    public int yAmt = 2;

    public GameObject waterParticle;

    private void Start() {
        StartCoroutine(spawn());
    }

    int count = 0;

    private IEnumerator spawn() {
        while(true) {
            for(float i = -((float)xAmt/2); i < (float)xAmt / 2; i++) {
                for(float j = -((float)yAmt/2); j < (float)yAmt/2; j++) {

                    float noise = Mathf.PerlinNoise(i/2.5f * 1.128392f * (count + 4) , j/5);

                    Instantiate(waterParticle, transform.position + new Vector3(i/2.5f + noise, j/5, 0), Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(0.1f);        
            
            count++;
        }
    }
}

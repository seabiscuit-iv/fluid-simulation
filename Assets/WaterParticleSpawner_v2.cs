
using System.Collections;
using UnityEngine;

public class WaterParticleSpawner_v2 : MonoBehaviour
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

                    float noise = Mathf.PerlinNoise(i / 14 * 234.123478f * ((count + 4) / 2.234f) , j/5);

                    GameObject obj = Instantiate(waterParticle, new Vector3(0f, 0f, 0f), Quaternion.identity);

                    obj. transform.position = 
                        transform.position + new Vector3((i/2 + (noise) * 3f - 1f), j/5 + noise * 5, 0);
                }
            }

            yield return new WaitForSeconds(0.5f);        
            
            count++;
        }
    }
}


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

    private IEnumerator spawn() {
        while(true) {
            for(float i = -((float)xAmt/2); i < (float)xAmt / 2; i++) {
                for(float j = -((float)yAmt/2); j < (float)yAmt/2; j++) {
                    Instantiate(waterParticle, transform.position + new Vector3(i/20, j/20, 0), Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(0.1f);        
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSenses : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        noiseMaker.noiseEvent += heardNoise;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void heardNoise(GameObject source, float volume, Vector3 position)
    {
        Debug.Log("heard a noise!");
    }
}

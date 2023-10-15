using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noiseMaker : MonoBehaviour
{
    public static Action<GameObject, float, Vector3> noiseEvent;
    public float volume = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void makeNoise()
    {
        noiseEvent.Invoke(gameObject, volume, gameObject.transform.position);
    }

}

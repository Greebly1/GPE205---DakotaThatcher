using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooter_Base : MonoBehaviour
{
    public noiseMaker noisecontroller;
    public Transform gunBarrel;
    public float damage;

    private void Awake()
    {
        noisecontroller = GetComponent<noiseMaker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Tries to shoot
    /// </summary>
    public abstract void tryShoot();
}

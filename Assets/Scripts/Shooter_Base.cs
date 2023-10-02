using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooter_Base : MonoBehaviour
{
    public Transform gunBarrel;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
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

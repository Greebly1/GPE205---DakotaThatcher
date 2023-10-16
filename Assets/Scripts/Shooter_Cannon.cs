using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter_Cannon : Shooter_Base
{
    public GameObject shell_prefab;
    public float fireCooldown = 1.0f;
    public float cannonVolume = 10.0f;

    private float FireCooldownTimer = 0.0f;
    
    // Start is called before the first frame update
    void Awake()
    {
        
        if (shell_prefab == null)
        {
            Debug.Log("A shooter_cannon does not have a shell prefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        FireCooldownTimer -= Time.deltaTime;  
    }

    /// <summary>
    /// Tries to fire this tank's cannon, if the fireCooldownTimer is <= 0 nothing happends
    /// </summary>
    public override void tryShoot()
    {
        if (canFire())
        {
            fire();
        }
    }
    private void fire()
    {
        restartCooldown();

        noise.noiseEvent.Invoke(new noise.sound(this.gameObject, cannonVolume, this.transform.position, Time.time));

        GameObject shell = Instantiate(shell_prefab, gunBarrel.position, gunBarrel.rotation);

        shell.GetComponent<Shell>().owner = gameObject;


    }
    public bool canFire()
    {
        return FireCooldownTimer < 0;
    }
    private void restartCooldown()
    {
        FireCooldownTimer = fireCooldown;
    }

    /// <summary>
    /// Gets the cooldown for firing tank cannon
    /// </summary>
    /// <returns>A 0-1 float</returns>
    public float getFireCooldown()
    {
        float clampedCooldown = Mathf.Clamp(FireCooldownTimer, 0f, fireCooldown);
        return clampedCooldown / fireCooldown;
    }
}

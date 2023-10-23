using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSenses : MonoBehaviour
{
        #region variables
    public float hearingRange = 50.0f;
    public float fieldOfView = 50.0f;
    public float sightRange = 100.0f;
    private noise.sound lastHeardSound;
    public Action heardSound = delegate { };


    #endregion

    #region Initialize

    private void Update()
    {
        //Debug.DrawLine(this.transform.position, this.transform.position + getTurretForward() * 25, Color.yellow);
    }
    void Awake()
    {
        noise.noiseEvent += handleNoiseEvent;
    }
    #endregion

    void handleNoiseEvent(noise.sound soundInfo)
    {
        if (soundInfo.volume + hearingRange > Vector3.Distance(soundInfo.location, this.gameObject.transform.position))
        {
            lastHeardSound = soundInfo;
            heardNoise();
        }
    }

    public bool canSee(GameObject target)
    {
        Vector3 forward = getTurretForward();
        Vector3 targetDirection = target.transform.position - this.transform.position;
        float targetAngle = Vector3.Angle(forward, targetDirection);
        if (targetAngle <= fieldOfView && Vector3.Distance(this.transform.position, target.transform.position) <= sightRange)
        {
            //Debug.Log("target within field of view");
            RaycastHit hitinfo;
            if (Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized, out hitinfo) && hitinfo.collider.gameObject == target)
            {
                //Debug.Log("sees the target");
                return true;
            }
        }
        return false;
    }

    private void heardNoise()
    {
        //Debug.Log("Heard a noise");
        heardSound.Invoke();
    }

        #region macros
    public float timeSinceSoundHeard()
    {
        return Time.time - lastHeardSound.time;
    }

    public Vector3 getTurretForward()
    {
        return this.gameObject.transform.up * -1;
    }
    #endregion
}

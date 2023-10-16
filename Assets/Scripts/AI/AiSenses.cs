using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSenses : MonoBehaviour
{
        #region variables
    public float hearingRange = 50.0f;
    private noise.sound lastHeardSound;

    
    #endregion

        #region Initialize
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

    private void heardNoise()
    {
        Debug.Log("Heard a noise");
    }

        #region macros
    public float timeSinceSoundHeard()
    {
        return Time.time - lastHeardSound.time;
    }
    #endregion
}

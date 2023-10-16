using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noise : MonoBehaviour
{
    public static Action<sound> noiseEvent = delegate { };
    public float volume = 10f;

    public struct sound
    {
        public readonly GameObject source;
        public readonly float volume;
        public readonly Vector3 location;
        public readonly float time;

        public sound(GameObject soundSource, float soundVolume, Vector3 soundLocation, float soundTime)
        {
            source = soundSource;
            volume = soundVolume;
            location = soundLocation;
            time = soundTime;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum sightState { seesEnemy, doesNotSeeEnemy}

public class AiSenses : MonoBehaviour
{
        #region variables
    public float hearingRange = 50.0f;
    public float fieldOfView = 50.0f;
    public float sightRange = 100.0f;
    private noise.sound lastHeardSound;
    public Action heardSound = delegate { };
    public Action<List<PlayerController>> sawEnemy = delegate { };
    public Action<List<PlayerController>> lostEnemy = delegate { };
    public List<PlayerController> targets = new List<PlayerController>();
    public int enemiesSeen = 0;

    public sightState _sightstate;
    #endregion

    #region Initialize

    private void Update()
    {
        targets = GameManager.Game.players.Where(player => player.pawn != null && canSee(player.pawn.gameObject)).ToList();

        if (targets.Count > enemiesSeen) {
            sawEnemy.Invoke(targets);
        } else if (targets.Count < enemiesSeen) {
            lostEnemy.Invoke(targets);
        }

        enemiesSeen = targets.Count;
    }
    void Awake()
    {
        _sightstate = sightState.doesNotSeeEnemy;
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

    public void OnDestroy()
    {
        noise.noiseEvent -= handleNoiseEvent;
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

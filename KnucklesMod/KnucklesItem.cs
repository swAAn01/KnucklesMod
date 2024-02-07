using KnucklesMod;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnucklesItem : GrabbableObject
{
    private static ILogger logger = Debug.unityLogger;

    public AudioClip scream; // sfx
    public AudioSource screamPlayer;

    private bool isScared;
    private bool isScreaming;
    private float noiseInterval;
    private RoundManager roundManager;
    private int timesPlayedWithoutTurningOff;

    private const int LAYERMASK = 1 << 19;

    public override void Start()
    {
        // GrabbableObject setup
        this.grabbable = true;
        this.isInFactory = true;
        base.Start();

        isScreaming = false;
        roundManager = FindObjectOfType<RoundManager>();

        screamPlayer.loop = true;
        screamPlayer.clip = scream;
        screamPlayer.volume = KnucklesPlugin.screamVolume.Value;
    }

    public override void Update()
    {
        base.Update();

        if (!KnucklesPlugin.screamNearEnemies.Value)
        {
            if (isScreaming)
            {
                screamPlayer.Stop();
                isScreaming = false;
                timesPlayedWithoutTurningOff = 0;
            }
            return;
        }

        screamPlayer.volume = KnucklesPlugin.screamVolume.Value;

        if (nearEnemy())
            isScared = true;
        else
            isScared = false;

        if (isScared && this.isHeld)
        {
            if (isScreaming)
            {
                if (noiseInterval <= 0f)
                {
                    noiseInterval = 1f;
                    timesPlayedWithoutTurningOff++;
                    roundManager.PlayAudibleNoise(base.transform.position, 16f, KnucklesPlugin.screamVolume.Value, timesPlayedWithoutTurningOff, noiseIsInsideClosedShip: false, 5);
                }
                else
                {
                    noiseInterval -= Time.deltaTime;
                }
            }
            else
            {
                screamPlayer.Play();
                isScreaming = true;
            }
        }
        else
        {
            if (isScreaming)
            {
                screamPlayer.Stop();
                isScreaming = false;
                timesPlayedWithoutTurningOff = 0;
            }
        }
    }

    private bool nearEnemy()
    {
        int init = base.gameObject.layer;
        base.gameObject.layer = 0;

        Vector3 pos = this.transform.position;

        bool result = Physics.CheckSphere(pos, KnucklesPlugin.enemyCheckRadius.Value, LAYERMASK);

        base.gameObject.layer = init;
        return result;
    }
}

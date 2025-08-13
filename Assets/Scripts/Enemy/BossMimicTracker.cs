using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMimicTracker : MonoBehaviour
{
    private MimicBossController boss;
    private bool registered;

    public void Attach(MimicBossController owner)
    {
        boss = owner;

        if (isActiveAndEnabled && !registered)
        {
            boss.RegisterMimic(this);
            registered = true;
        }
    }

    private void OnEnable()
    {
        if (boss != null && !registered)
        {
            boss.RegisterMimic(this);
            registered = true;
        }
    }

    private void OnDisable()
    {
        if (registered)
        {
            boss.UnregisterMimic(this);
            registered = false;
        }
    }

    private void OnDestroy()
    {
        if (registered)
        {
            boss.UnregisterMimic(this);
            registered = false;
        }
    }
}


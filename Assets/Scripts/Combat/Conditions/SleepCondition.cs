﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// When added to a creature, will put it to sleep for a specified duration, then waking it up.
/// If it takes damage, it is woken up immediately.
/// </summary>
class SleepCondition: StunCondition
{
    private float originalHitpoints;
    protected override void Start()
    {
        originalHitpoints = GetComponent<CombatantBase>().HitPoints;
        base.Start();
    }

    protected override void Update()
    {
        if (originalHitpoints != GetComponent<CombatantBase>().HitPoints)
        {
            // Took damage, wake up.
            EndCondition();
            return;
        }
        base.Update();
    }

    protected override void StartCondition()
    {
        base.StartCondition();
        GetComponent<Animator>().SetBool("Asleep", true);
    }

    protected override void EndCondition()
    {
        base.EndCondition();
        GetComponent<Animator>().SetBool("Asleep", false);
    }
}
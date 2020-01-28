﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : CombatantBase
{
    public MonsterRole Role;
    public MonsterRank Rank;
    protected override void Awake()
    {
        base.Awake();
        combatantsManager.Enemies.Add(this);
        DamageMaxHitPointsDirectly = true;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int damage, CombatantBase FromCombatant)
    {
        base.TakeDamage(damage, FromCombatant);
        if (IsDown)
        {
            combatantsManager.Enemies.Remove(this);
        }
    }
}

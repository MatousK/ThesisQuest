﻿using System;
using Assets.Scripts.Effects.Projectiles;
using UnityEngine;

namespace Assets.Scripts.Combat.Skills
{
    public class ProjectileAttack : Attack
    {
        /// <summary>
        /// Projectile that should be spawned when the attack is started. Should be subobject of the character/monster and should be in the appropriate position relative to the monster.
        /// </summary>
        public GameObject ProjectileTemplate;
        // How many squares should the projectile traverse per second.
        public float ProjectileSpeed;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void ApplySkillEffects(object sender, EventArgs e)
        {
            var newProjectile = Instantiate(ProjectileTemplate, transform, true);
            newProjectile.transform.parent = null;
            newProjectile.SetActive(true);
            // Target could change before the projectile hits, so put it in a variable just to be sure.
            var originalTarget = Target;
            newProjectile.GetComponent<Projectile>().StartProjectile(Target, ProjectileSpeed, () => OnProjectileHit(originalTarget));
            base.ApplySkillEffects(sender, e);
        }

        protected virtual void OnProjectileHit(CombatantBase originalTarget)
        {
            originalTarget.TakeDamage(DamagePerHit, SelfCombatant);
            CombatSoundsController.ProjectileHit(SkillSoundType);
        }
    }
}
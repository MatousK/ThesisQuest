﻿using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI.CharacterPortrait
{
    public class CharacterPortraitsManager: MonoBehaviour
    {
        public List<CharacterPortrait> AvailablePortraitWidgets;
        private CombatantsManager combatantsManager;

        
        void Update()
        {
            if (combatantsManager == null)
            {
                combatantsManager = FindObjectOfType<CombatantsManager>();
                if (combatantsManager == null)
                {
                    // Game is probably not loaded yet;
                    return;
                }
            }
            UpdatePortraits();
        }

        void UpdatePortraits()
        {
            var currentPartyMembers = combatantsManager.PlayerCharacters;
            for (int i = 0; i < AvailablePortraitWidgets.Count; ++i)
            {
                if (i < currentPartyMembers.Count)
                {
                    AvailablePortraitWidgets[i].gameObject.SetActive(true);
                    AvailablePortraitWidgets[i].RepresentedHero = currentPartyMembers[i];
                }
                else
                {
                    AvailablePortraitWidgets[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
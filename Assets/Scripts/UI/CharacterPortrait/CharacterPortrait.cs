﻿using System.Linq;
using Assets.Scripts.Camera;
using Assets.Scripts.Combat;
using Assets.Scripts.Sound.CharacterSounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.CharacterPortrait
{
    public class CharacterPortrait : MonoBehaviour, IPointerClickHandler
    {
        CombatantsManager combatantsManager;

        public Hero RepresentedHero;

        public AttributeField AttackField;
        public AttributeField DefenseField;
        public AttributeField MaxHealthField;

        public Gradient PortraitBackgroundGradient;
        public Image ImageBackground;
        public Image PortraitImage;
        public Image MaxHealthIndicator;
        public Image CurrentHealthIndicator;
        public Image Border;

        // Used to detect doubleClick
        private float lastPortraitClickTime;
        private const float DoubleClickTime = 0.25f;

        private CameraMovement cameraMovement;
        private CharacterVoiceController heroVoiceController;

        void Awake()
        {
            combatantsManager = FindObjectOfType<CombatantsManager>();
            cameraMovement = FindObjectOfType<CameraMovement>();
        }

        public void Update()
        {
            if (RepresentedHero == null)
            {
                heroVoiceController = null;
                return;
            }

            heroVoiceController = RepresentedHero.GetComponentInChildren<CharacterVoiceController>();
            PortraitImage.sprite = RepresentedHero.Portrait;

            AttackField.ValueToShow = (int)RepresentedHero.Attributes.DealtDamageMultiplier;
            DefenseField.ValueToShow = (int)((1 - RepresentedHero.Attributes.ReceivedDamageMultiplier) * 100);
            MaxHealthField.ValueToShow = (int)RepresentedHero.TotalMaxHitpoints;

            var maxHealthPercentage = RepresentedHero.MaxHitpoints / RepresentedHero.TotalMaxHitpoints;
            var currentHealthPercentage = RepresentedHero.HitPoints / RepresentedHero.TotalMaxHitpoints;
            var totalHealthPercentage = (currentHealthPercentage + maxHealthPercentage) / 2;

            ImageBackground.color = PortraitBackgroundGradient.Evaluate(1 - totalHealthPercentage);

            MaxHealthIndicator.rectTransform.anchorMax = new Vector2(maxHealthPercentage, MaxHealthIndicator.rectTransform.anchorMax.y);

            CurrentHealthIndicator.rectTransform.anchorMax = new Vector2(currentHealthPercentage, CurrentHealthIndicator.rectTransform.anchorMax.y);

            var isSelected = RepresentedHero.GetComponent<SelectableObject>().IsSelected;
            Border.color = isSelected ? Color.green : Color.white;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Time.realtimeSinceStartup - lastPortraitClickTime < DoubleClickTime)
                {
                    // Double click detected.
                    cameraMovement.QuickFindHero(RepresentedHero);
                }
                else
                {
                    lastPortraitClickTime = Time.realtimeSinceStartup;
                    heroVoiceController.PlayOnSelectedSound();
                    SelectHero();
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                DoActionOnHero();
            }
        }

        void SelectHero()
        {
            RepresentedHero.GetComponent<SelectableObject>().IsSelected = true;
            if (!UnityEngine.Input.GetKey(KeyCode.LeftShift) && !UnityEngine.Input.GetKey(KeyCode.RightShift))
            {
                // If not holding shift, deselect, other characters.
                foreach (var playerCharacter in combatantsManager.PlayerCharacters)
                {
                    if (playerCharacter != RepresentedHero)
                    {
                        playerCharacter.GetComponent<SelectableObject>().IsSelected = false;
                    }
                }
            }
        }

        void DoActionOnHero()
        {
            var usingSkill = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
            var selectedCharacters = combatantsManager.GetPlayerCharacters(onlySelected: true).ToList();
            var speakingHero = selectedCharacters.FirstOrDefault();
            var speakingCharacterVoice = speakingHero != null
                ? speakingHero.GetComponentInChildren<CharacterVoiceController>()
                : null;
            VoiceOrderType? voiceOrderType = null;
            foreach (var hero in selectedCharacters)
            {
                if (hero == RepresentedHero)
                {
                    if (usingSkill)
                    {
                        if (hero == speakingHero)
                        {
                            voiceOrderType = VoiceOrderType.SelfSkill;
                        }
                        hero.SelfSkillUsed();
                    }
                    else
                    {
                        hero.SelfClicked();
                    }
                }
                else
                {
                    if (usingSkill)
                    {
                        if (hero == speakingHero)
                        {
                            voiceOrderType = VoiceOrderType.FriendlySkill;
                        }
                        hero.FriendlySkillUsed(RepresentedHero);
                    }
                    else
                    {
                        hero.FriendlyClicked(RepresentedHero);
                    }
                }
            }

            if (voiceOrderType != null && speakingCharacterVoice != null)
            {
                speakingCharacterVoice.OnOrderGiven(voiceOrderType.Value);
            }
        }
    }
}
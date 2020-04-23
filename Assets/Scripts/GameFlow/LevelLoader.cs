﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.Scripts.Combat;
using Assets.Scripts.CombatSimulator;
using Assets.Scripts.Extension;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameFlow
{
    class LevelLoader: MonoBehaviour
    {
        public LevelDefinition[] StoryModeLevels;

        public LevelDefinition FreePlayLevel;
        public PartyConfiguration CurrentPartyConfiguration;
        public GameObject LevelIntroScreenTemplate;
        [HideInInspector]
        public bool IsPendingKill;
        [HideInInspector]
        public LevelGraph CurrentLevelGraph;

        public SceneType CurrentSceneType;
        private int currentStoryModeLevelIndex;
        private bool isPlayingStoryMode;
        private Animation animationComponent;
        private string nextLevelSceneName;
        

        private void Awake()
        {
            if (FindObjectsOfType<LevelLoader>().Length > 1)
            {
                Destroy(gameObject);
                IsPendingKill = true;
            }
            DontDestroyOnLoad(gameObject);
            animationComponent = GetComponent<Animation>();
        }

        public void StartStoryMode()
        {
            currentStoryModeLevelIndex = 0;
            isPlayingStoryMode = true;
            LoadLevelWithIntro(StoryModeLevels.First());
        }

        public void StartFreeMode()
        {
            isPlayingStoryMode = false;
            LoadLevelWithIntro(FreePlayLevel);
        }

        public void LoadNextLevel()
        {
            if (isPlayingStoryMode)
            {
                if (++currentStoryModeLevelIndex >= StoryModeLevels.Length)
                {
                    ShowVictoryScreen();
                }
                else
                {
                    LoadLevelWithIntro(StoryModeLevels[currentStoryModeLevelIndex]);
                }
            }
            else
            {
                LoadLevelWithIntro(FreePlayLevel);
            }
        }

        public void ShowVictoryScreen()
        {

        }
        public void LoadLevelWithIntro(LevelDefinition level)
        {
            if (level != null && (level.IntroTexts?.Any() == true || !string.IsNullOrEmpty(level.SurveyLink)))
            {
                var introScreenObject = Instantiate(LevelIntroScreenTemplate);
                var levelIntro = introScreenObject.GetComponentInChildren<TypewriterWithSurveyScreen>();
                levelIntro.LevelDefinition = level;
            }
            else
            {
                LoadLevel(level);
            }
        }

        public void LoadLevel(LevelDefinition level)
        {
            // If loading next floor, store the hero attributes.
            var existingHeroes = FindObjectsOfType<Hero>();
            CurrentPartyConfiguration = existingHeroes.Any() ? new PartyConfiguration(existingHeroes.ToArray()) : null;
            CurrentSceneType = level.Type;
            switch (level.Type)
            {
                case SceneType.DungeonLevel:
                    nextLevelSceneName = "DungeonScene";
                    break;
                case SceneType.Credits:
                    nextLevelSceneName = "Credits";
                    break;
                case SceneType.MainMenu:
                    nextLevelSceneName = "MainMenu";
                    break;
            }
            CurrentLevelGraph = level.PossibleLevelGraphs.GetRandomElementOrDefault();
            animationComponent.Play();
        }

        public void FadeOutDone()
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
    }
}

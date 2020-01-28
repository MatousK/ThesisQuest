﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EncounterGenerator
{
    public List<GameObject> GenerateEncounters(EncounterConfiguration configuration)
    {
        var targetDifficulty = configuration.EncounterDifficulty.GetDifficultyForPartyStrength(0);
        var generationParams = new GenerateMonsterGroupParameters();
        generationParams.TargetEncounterDifficulty = targetDifficulty;
        generationParams.MonsterPriorities = new Dictionary<GameObject, float>();
        var testMonsterTypeDefinition = new MonsterTypeDefinition(MonsterRank.Regular, MonsterRole.Brute);
        generationParams.RequestedMonsters = new List<KeyValuePair<MonsterTypeDefinition, int>> {
            new KeyValuePair<MonsterTypeDefinition, int>(testMonsterTypeDefinition, (int)(targetDifficulty))
            };
        return configuration.MonsterGroupDefinition.First().GenerateMonsterGroup(generationParams);
    }
}
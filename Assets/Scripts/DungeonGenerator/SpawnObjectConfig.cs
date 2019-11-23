﻿using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Spawn Object", fileName = "SpawnObject")]
public class SpawnObjectConfig : PipelineConfig
{
    public GameObject ObjectToSpawn;
    public Vector3 ObjectPositon;
}
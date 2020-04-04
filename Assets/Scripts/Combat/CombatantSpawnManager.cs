﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CombatantSpawnManager: MonoBehaviour
{
    // Used so we know where can we spawn monsters.
    PathfindingMapController pathfindingMapController;

    private void Awake()
    {
        pathfindingMapController = FindObjectOfType<PathfindingMapController>();
    }
    /// <summary>
    /// Spawns a monster on the map.
    /// </summary>
    /// <param name="combatantTemplate">The monster that should be spawned.</param>
    /// <param name="spawnRoom">The room in which the monster should be spawned.</param>
    /// <param name="incomingDoors">Doors through which the party came.</param>
    /// <param name="minDistanceToDoor">Minimum distance from the doors to spawn</param>
    /// <returns></returns>
    public GameObject SpawnCombatant(GameObject combatantTemplate, RoomInfo spawnRoom, Doors incomingDoors = null, float minDistanceToDoor = 0)
    {
        //This will get the map in which positions of other combatants are also marked as impassable.
        var spawnTileCandidates = new List<Vector2Int>(spawnRoom.RoomSquaresPositions);
        return SpawnCombatant(combatantTemplate, spawnTileCandidates, incomingDoors, minDistanceToDoor);
    }
    /// <summary>
    /// Spawns a monster on the map.
    /// </summary>
    /// <param name="combatantTemplate">The monster that should be spawned.</param>
    /// <param name="spawnRoom">The room in which the monster should be spawned.</param>
    /// <param name="incomingDoors">Doors through which the party came.</param>
    /// <param name="minDistanceToDoor">Minimum distance from the doors to spawn</param>
    /// <returns></returns>
    public GameObject SpawnCombatant(GameObject combatantTemplate, List<Vector2Int> tiles, Doors incomingDoors = null, float minDistanceToDoor = 0)
    {
        var random = new System.Random();
        //This will get the map in which positions of other combatants are also marked as impassable.
        // TODO: Make this behavior more understandable
        var pathfindingMap = pathfindingMapController.GetPassabilityMapForCombatant(null);
        while (tiles.Any())
        {
            var spawnTileCandidateIndex = random.Next(tiles.Count);
            var spawnTileCandidate = tiles[spawnTileCandidateIndex];
            var distanceToDoor = incomingDoors == null ? float.PositiveInfinity : Vector2.Distance(incomingDoors.transform.localPosition, spawnTileCandidate);
            bool isTooClose = distanceToDoor < minDistanceToDoor;
            if (!pathfindingMap.GetSquareIsPassable(spawnTileCandidate) || isTooClose)
            {
                tiles.RemoveAt(spawnTileCandidateIndex);
                continue;
            }
            var combatantInstance = Instantiate(combatantTemplate);
            combatantInstance.transform.parent = transform;
            combatantInstance.transform.localPosition = new Vector3Int(spawnTileCandidate.x, spawnTileCandidate.y, -1);
            combatantInstance.SetActive(true);
            return combatantInstance;
        }
        return null;
    }
}
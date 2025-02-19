﻿using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Football.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
        {
            if (isMtf) return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ElementAt(0).transform.position;
            else return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ElementAt(1).transform.position;
        }
    }
}

﻿using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Knives.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isScientist)
        {
            if (isScientist) return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").FirstOrDefault().transform.position;
            else return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint1").FirstOrDefault().transform.position;
        }
    }
}

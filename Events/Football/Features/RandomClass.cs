﻿using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Football.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
        {
            if (isMtf) return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").FirstOrDefault().transform.position;
            else return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint1").FirstOrDefault().transform.position;
        }
    }
}

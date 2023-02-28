﻿using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AutoEvent.Events
{
    internal class DeathmatchEvent : IEvent
    {
        public string Name => "Территория Смерти";
        public string Description => "Крутой deathmatch из mw19";
        public string Color => "FFFF00";
        public string CommandName => "deathmatch";
        public TimeSpan EventTime { get; set; }
        public static SchematicObject GameMap { get; set; }
        public static List<Vector3> Spawners { get; set; } = new List<Vector3>();
        public static int MtfKills;
        public static int ChaosKills;
        public int NeedKills;

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += DeathmatchHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += DeathmatchHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Died += DeathmatchHandler.OnDied;
            Exiled.Events.Handlers.Player.SpawningRagdoll += DeathmatchHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned += DeathmatchHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole += DeathmatchHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += DeathmatchHandler.OnPlaceBlood;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= DeathmatchHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= DeathmatchHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Died -= DeathmatchHandler.OnDied;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= DeathmatchHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= DeathmatchHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= DeathmatchHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= DeathmatchHandler.OnPlaceBlood;

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("Shipment", new Vector3(120f, 1020f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            Extensions.PlayAudio("ClassicMusic.ogg", 3, true, "Deathmatch");

            MtfKills = 0;
            ChaosKills = 0;
            // Choosing the number of kills for the end of the mini-game
            for (int i = 0; i < Player.List.Count(); i += 5)
            {
                NeedKills += 15;
            }

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(DeathmatchClass.GetRandomClass(Team.FoundationForces));
                    player.CurrentItem = player.Items.ElementAt(1);
                    player.Position = GameMap.Position + DeathmatchRandom.GetRandomPosition();
                }
                else
                {
                    player.Role.Set(DeathmatchClass.GetRandomClass(Team.ChaosInsurgency));
                    player.CurrentItem = player.Items.ElementAt(1);
                    player.Position = GameMap.Position + DeathmatchRandom.GetRandomPosition();
                }
                player.EnableEffect<CustomPlayerEffects.Scp1853>(30);
                player.EnableEffect(EffectType.MovementBoost, 30);
                player.ChangeEffectIntensity(EffectType.MovementBoost, 25);
                player.EnableEffect<CustomPlayerEffects.SpawnProtected>(10);
                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "deathmatch_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
            // If you need to stop the game, then just kill all the players
            while (MtfKills < NeedKills && ChaosKills < NeedKills && Player.List.Count(r => r.IsAlive) > 0)
            {
                string mtfString = string.Empty;
                string chaosString = string.Empty;
                for (int i = 0; i < NeedKills; i += (int)(NeedKills / 5))
                {
                    if (MtfKills >= i) mtfString += "■";
                    else mtfString += "□";

                    if (ChaosKills >= i) chaosString = "■" + chaosString;
                    else chaosString = "□" + chaosString;
                }
                Extensions.Broadcast($"<color=#D71868><b><i>Территория Смерти</i></b></color>\n" +
                    $"<b><color=yellow><color=#42AAFF> {MtfKills} {mtfString}> </color> <color=red>|</color> <color=green> <{chaosString} {ChaosKills}</color></color></b>", 1);

                yield return Timing.WaitForSeconds(1f);
            }
            if (MtfKills == NeedKills)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Территория Смерти</i></b></color>\n" +
                    $"<color=yellow>ПОБЕДИТЕЛИ: <color=green>ХАОС</color></color>", 10);
            }
            else if (ChaosKills == NeedKills)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Территория Смерти</i></b></color>\n" +
                    $"<color=yellow>ПОБЕДИТЕЛИ: <color=#42AAFF>МОГ</color></color>", 10);
            }
            else
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Территория Смерти</i></b></color>\n" +
                    $"<color=yellow>Игра была приостановлена Администратором.</color>", 10);
            }
            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}

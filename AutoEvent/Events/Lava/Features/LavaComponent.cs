﻿using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Events.Lava.Features
{
    public class LavaComponent : MonoBehaviour
    {
        private BoxCollider collider;
        private float damageCooldown = 3f;
        private float elapsedTime = 0f;

        private void Start()
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
        }

        private void OnTriggerStay(Collider other)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= damageCooldown)
            {
                elapsedTime = 0f;

                if (Player.Get(other.gameObject) is Player)
                {
                    var pl = Player.Get(other.gameObject);
                    pl.Hurt(30, $"{AutoEvent.Singleton.Translation.PuzzleDied}");
                }
            }
        }
    }
}

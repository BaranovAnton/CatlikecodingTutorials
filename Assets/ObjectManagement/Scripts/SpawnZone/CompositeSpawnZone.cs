﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectManagement.Scripts
{
    public class CompositeSpawnZone : SpawnZone
    {
        [SerializeField] private bool sequential;     
        [SerializeField] SpawnZone[] spawnZones;
        
        private int nextSequentialIndex;
        
        public override Vector3 SpawnPoint 
        {
            get
            {
                int index;
                if (sequential)
                {
                    index = nextSequentialIndex++;
                    if (nextSequentialIndex >= spawnZones.Length)
                    {
                        nextSequentialIndex = 0;
                    }
                }
                else
                {
                    index = Random.Range(0, spawnZones.Length);
                }
                
                return spawnZones[index].SpawnPoint;
            }
        }
        
        public override void Save (GameDataWriter writer) {
            writer.Write(nextSequentialIndex);
        }

        public override void Load (GameDataReader reader) {
            nextSequentialIndex = reader.ReadInt();
        }
    }
}
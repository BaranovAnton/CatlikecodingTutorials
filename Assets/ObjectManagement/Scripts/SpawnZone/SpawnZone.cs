using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectManagement.Scripts
{
    public abstract class SpawnZone : MonoBehaviour
    {
        public abstract Vector3 SpawnPoint
        {
            get;
        }
    }
}
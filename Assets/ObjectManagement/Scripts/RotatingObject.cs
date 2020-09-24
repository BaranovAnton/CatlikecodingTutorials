using System.Collections;
using System.Collections.Generic;
using ObjectManagement.Scripts;
using UnityEngine;

namespace ObjectManagement.Scripts
{
    public class RotatingObject : PersistableObject
    {
        [SerializeField] private Vector3 angularVelocity;

        void FixedUpdate () {
            transform.Rotate(angularVelocity * Time.deltaTime);
        }
    }
}
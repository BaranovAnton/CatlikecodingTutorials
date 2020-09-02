using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    int shapeId = int.MinValue;

    public int MaterialId { get; private set; }

    public int ShapeId 
    { 
        get => shapeId; 
        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            } else
            {
                Debug.LogError("Not allowed to change shapeId");
            }
        } 
    }

    public void SetMaterial(Material material, int materialId) 
    {
        GetComponent<MeshRenderer>().material = material;
        MaterialId = materialId;
    }
}

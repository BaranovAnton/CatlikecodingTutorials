using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectManagement.Scripts
{
    public class Shape : PersistableObject
    {
        int shapeId = int.MinValue;

        private Color color;
        
        public int MaterialId { get; private set; }

        public int ShapeId
        {
            get => shapeId;
            set
            {
                if (shapeId == int.MinValue && value != int.MinValue)
                {
                    shapeId = value;
                }
                else
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

        public void SetColor(Color color)
        {
            this.color = color;
            GetComponent<MeshRenderer>().material.color = color;
        }

        public override void Save(GameDataWriter writer)
        {
            base.Save(writer);
            writer.Write(color);
        }

        public override void Load(GameDataReader reader)
        {
            base.Load(reader);
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
    }
}
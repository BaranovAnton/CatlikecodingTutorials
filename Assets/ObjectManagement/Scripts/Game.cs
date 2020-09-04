using System.Collections.Generic;
using UnityEngine;

namespace ObjectManagement.Scripts 
{
    public class Game : PersistableObject
    {
        public PersistentStorage storage;
        public ShapeFactory shapeFactory;
        public KeyCode keyCode = KeyCode.C;
        public KeyCode newGameCode = KeyCode.N;
        public KeyCode saveKey = KeyCode.S;
        public KeyCode loadKey = KeyCode.L;

        private List<Shape> shapes;

        const int saveVersion = 1;

        private void Awake()
        {
            shapes = new List<Shape>();
        }

        void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                CreateShape();
            } else if (Input.GetKeyDown(newGameCode))
            {
                BeginNewGame();
            } else if (Input.GetKeyDown(saveKey))
            {
                storage.Save(this, saveVersion);
            } else if (Input.GetKeyDown(loadKey))
            {
                BeginNewGame();
                storage.Load(this);
            }
        }

        private void CreateShape()
        {
            Shape instance = shapeFactory.GetRandom();
            Transform t = instance.transform;

            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
            instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
            shapes.Add(instance);
        }

        private void BeginNewGame()
        {
            for (int i=0; i<shapes.Count; i++)
            {
                Destroy(shapes[i].gameObject);
            }
            shapes.Clear();
        }

        public override void Save(GameDataWriter writer)
        {
            writer.Write(shapes.Count);
            for (int i = 0; i < shapes.Count; i++)
            {
                writer.Write(shapes[i].ShapeId);
                writer.Write(shapes[i].MaterialId);
                shapes[i].Save(writer);
            }
        }

        public override void Load(GameDataReader reader)
        {
            int version = reader.Version;
            if (version > saveVersion)
            {
                Debug.LogError("Unsupported future save version " + version);
                return;
            }

            int count = (version <= 0)? -version : reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int shapeId = version > 0 ? reader.ReadInt() : 0;
                int materialId = version > 0 ? reader.ReadInt() : 0;
                Shape instance = shapeFactory.Get(shapeId, materialId);
                instance.Load(reader);
                shapes.Add(instance);
            }
        }
    }
}
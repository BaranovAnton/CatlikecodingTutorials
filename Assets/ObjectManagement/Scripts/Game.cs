using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Game : PersistableObject
{
    public PersistentStorage storage;
    public ShapeFactory shapeFactory;
    public KeyCode keyCode = KeyCode.C;
    public KeyCode newGameCode = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    private List<Shape> shapes;

    private void Awake()
    {
        shapes = new List<Shape>();
    }

    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            CreateObject();
        } else if (Input.GetKeyDown(newGameCode))
        {
            BeginNewGame();
        } else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this);
        } else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }

    private void CreateObject()
    {
        PersistableObject o = Instantiate(prefab);
        Transform t = o.transform;

        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
        shapes.Add(o);
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
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            shapes.Add(o);
        }
    }
}

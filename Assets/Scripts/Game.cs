using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Game : PersistableObject
{
    public PersistentStorage storage;
    public PersistableObject prefab;
    public KeyCode keyCode = KeyCode.C;
    public KeyCode newGameCode = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    private List<PersistableObject> objects;

    private void Awake()
    {
        objects = new List<PersistableObject>();
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
        objects.Add(o);
    }

    private void BeginNewGame()
    {
        for (int i=0; i<objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }
}

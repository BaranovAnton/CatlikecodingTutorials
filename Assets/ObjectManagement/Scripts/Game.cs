using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace ObjectManagement.Scripts 
{
    public class Game : PersistableObject
    {
        public static Game Instance { get; private set; }
    
        [SerializeField] PersistentStorage storage;
        [SerializeField] ShapeFactory shapeFactory;
        [SerializeField] KeyCode createKey = KeyCode.C;
        [SerializeField] KeyCode destroyKey = KeyCode.X;
        [SerializeField] KeyCode newGameCode = KeyCode.N;
        [SerializeField] KeyCode saveKey = KeyCode.S;
        [SerializeField] KeyCode loadKey = KeyCode.L;
        [SerializeField] int levelCount;
        
        [SerializeField] bool reseedOnLoad;
        public SpawnZone SpawnZoneOfLevel { get; set; }
        
        private int loadedLevelBuildIndex;
        
        private Random.State mainRandomState;

        public float CreationSpeed { get; set; }
        public float DestructionSpeed { get; set; }
        private float creationProgress, destructionProgress;

        private List<Shape> shapes;

        const int saveVersion = 3;

        void OnEnable () 
        {
            Instance = this;
        }
        
        private void Start()
        {
            Instance = this;
            
            mainRandomState = Random.state;
            shapes = new List<Shape>();

            if (Application.isEditor)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene loadedLevel = SceneManager.GetSceneAt(i);
                    if (loadedLevel.name.Contains("Level "))
                    {
                        SceneManager.SetActiveScene(loadedLevel);
                        loadedLevelBuildIndex = loadedLevel.buildIndex;
                        return;
                    }
                }
            }
            
            BeginNewGame();
            StartCoroutine(LoadLevel(1));
        }

        void Update()
        {
            if (Input.GetKeyDown(createKey))
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
            } else if (Input.GetKeyDown(destroyKey))
            {
                DestroyShape();
            }
            else
            {
                for (int i = 1; i <= levelCount; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                    {
                        BeginNewGame();
                        StartCoroutine(LoadLevel(i));
                        return;
                    }
                }
            }
            
            creationProgress += Time.deltaTime * CreationSpeed;
            while (creationProgress >= 1f)
            {
                creationProgress -= 1f;
                CreateShape();
            }
            
            destructionProgress += Time.deltaTime * DestructionSpeed;
            while (destructionProgress >= 1f)
            {
                destructionProgress -= 1f;
                DestroyShape();
            }
        }
        
        private IEnumerator LoadLevel (int levelBuildIndex)
        {
            enabled = false;
            if (loadedLevelBuildIndex > 0) {
                yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
            }
            yield return SceneManager.LoadSceneAsync(
                levelBuildIndex, LoadSceneMode.Additive
            );
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
            loadedLevelBuildIndex = levelBuildIndex;
            enabled = true;
        }

        private void CreateShape()
        {
            Shape instance = shapeFactory.GetRandom();
            Transform t = instance.transform;

            t.localPosition = SpawnZoneOfLevel.SpawnPoint;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * Random.Range(0.1f, 1.0f);
            instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
            shapes.Add(instance);
        }

        private void BeginNewGame()
        {
            Random.state = mainRandomState;
            int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
            mainRandomState = Random.state;
            Random.InitState(seed);
            
            for (int i=0; i<shapes.Count; i++)
            {
                shapeFactory.Reclaim(shapes[i]);
            }
            shapes.Clear();
        }

        public override void Save(GameDataWriter writer)
        {
            writer.Write(shapes.Count);
            writer.Write(Random.state);
            writer.Write(loadedLevelBuildIndex);
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
            if (version >= 3) {
                Random.State state = reader.ReadRandomState();
                if (!reseedOnLoad) 
                {
                    Random.state = state;
                }
            }
            StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
            
            for (int i = 0; i < count; i++)
            {
                int shapeId = version > 0 ? reader.ReadInt() : 0;
                int materialId = version > 0 ? reader.ReadInt() : 0;
                Shape instance = shapeFactory.Get(shapeId, materialId);
                instance.Load(reader);
                shapes.Add(instance);
            }
        }
        
        private void DestroyShape()
        {
            if (shapes.Count > 0)
            {
                int index = Random.Range(0, shapes.Count);
                shapeFactory.Reclaim(shapes[index]);
                int lastIndex = shapes.Count - 1;
                shapes[index] = shapes[lastIndex];
                shapes.RemoveAt(lastIndex);
            }
        }
    }
}
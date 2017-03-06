using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

    public static float TreeDensity = 0.005f;
    private static float FenceDensity = 0.001f;
    private static float GrassDensity = 0.05f;
    private static float RememberTreeDensity = TreeDensity; // Temporary cheat

    private const float ChunkWidthRadius = 150; // X axis
    private const float ChunkLength = 2;  // Z axis

    private const float BeginChunkZ = 10f;
    private const float StartDespawnZ = -10f;
    private const float MinimumRenderDistanceZ = 120f; // The minimum distance (from the player) that the furthest chunk is spawned.

    private float DistanceTraveledBeforeDifficultyIncrease = 100f;
    private float DifficultyIncreaseForTrees = 0.00005f;
    private float Difficulty = 1f;

    class SpawnableGameObject {
        public GameObject ParentObject;
        public GameObject Resource;
        public Action<SpawnableGameObject, Vector3, List<GameObject>> SpawnFunc;

        public SpawnableGameObject(string resourceName, Action<SpawnableGameObject, Vector3, List<GameObject>> newSpawnFunc) {
            ParentObject = new GameObject();
            ParentObject.name = resourceName;
            this.Resource = (GameObject)Resources.Load(resourceName);
            this.SpawnFunc = newSpawnFunc;
        }
    }

    class SpawnableGroup : SpawnableGameObject {
        public Func<float> Density;

        public SpawnableGroup(string resourceName, Action<SpawnableGameObject, Vector3, List<GameObject>> newSpawnFunc, Func<float> newDensity) : base(resourceName, newSpawnFunc) {
            this.Density = newDensity;
        }
    }

    private static System.Random rand = new System.Random();

    class ChunkTemplate {
        public float TotalDensity;

        public List<SpawnableGroup> SpawnTypes;

        public ChunkTemplate(List<SpawnableGroup> newSpawnTypes) {
            SpawnTypes = newSpawnTypes;
            TotalDensity = getTotalDensity();
        }

        private float getTotalDensity() {
            float density = 0;
            for (int i = 0; i < SpawnTypes.Count; i++) {
                density += SpawnTypes[i].Density();
            }
            return density;
        }
    }

    class Chunk {
        private float SpawnCount;
        public Rect SpawnArea;

        private List<GameObject> Spawned = new List<GameObject>();

        public Chunk(ChunkTemplate chunkTemplate, Rect newSpawnArea) {
            SpawnArea = newSpawnArea;
            float spawnSurface = SpawnArea.width * SpawnArea.height;
            SpawnCount = spawnSurface * chunkTemplate.TotalDensity;
            for (int i = 0; i < SpawnCount; i++) {
                Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(SpawnArea.xMin, SpawnArea.xMax), 0, UnityEngine.Random.Range(SpawnArea.yMin, SpawnArea.yMax));

                // Spawn a random GameObject in the average of the density of all combined GameObjects.
                double randNum = rand.NextDouble() * chunkTemplate.TotalDensity;
                for (int i2 = 0; i2 < chunkTemplate.SpawnTypes.Count; i2++) {
                    randNum -= chunkTemplate.SpawnTypes[i2].Density();
                    if (randNum < 0) {
                        chunkTemplate.SpawnTypes[i2].SpawnFunc(chunkTemplate.SpawnTypes[i2], randomPosition, Spawned);
                        break;
                    }
                }
            }
        }

        public void RemoveChunk() {
            for (int i = Spawned.Count - 1; i >= 0; i--) {
                UnityEngine.Object.DestroyImmediate(Spawned[i]);
                Spawned.RemoveAt(i);
            }
        }
    }

    static void spawnTree(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;

    }
    public static void spawnFenceFunc(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(Vector3.zero));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    public static void spawnGrassFunc(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    public static void spawnPlaneFunc(ChunkTemplate template, Rect spawnArea, List<GameObject> Spawned) {
        GameObject obj = Instantiate(template.Plane, new Vector3(spawnArea.center.x, 0, spawnArea.center.y), Quaternion.identity);
        obj.transform.localScale = new Vector3(spawnArea.width / 10, 1, spawnArea.height / 10);
        Spawned.Add(obj);
        obj.transform.parent = template.PlaneParent.transform;
    }
}

public class SpawnableGameObject {
    public GameObject ParentObject;
    public GameObject Resource;
    public string resourceName;
    public Action<SpawnableGameObject, Vector3, List<GameObject>> SpawnFunc;

    public SpawnableGameObject(string newResourceName, Action<SpawnableGameObject, Vector3, List<GameObject>> newSpawnFunc) {
        ParentObject = new GameObject();
        ParentObject.name = newResourceName;
        this.Resource = (GameObject)Resources.Load(newResourceName);
        this.SpawnFunc = newSpawnFunc;
        this.resourceName = newResourceName;
    }
}

public class SpawnableGroup : SpawnableGameObject {
    public Func<float> Density;

    public SpawnableGroup(string resourceName, Action<SpawnableGameObject, Vector3, List<GameObject>> newSpawnFunc, Func<float> newDensity) : base(resourceName, newSpawnFunc) {
        this.Density = newDensity;
    }
}

public class ChunkTemplate {
    public float TotalDensity;

    public List<SpawnableGroup> SpawnTypes;
    public GameObject Plane;
    public GameObject PlaneParent;

    public ChunkTemplate(List<SpawnableGroup> newSpawnTypes, GameObject plane) {
        SpawnTypes = newSpawnTypes;
        TotalDensity = getTotalDensity();
        this.Plane = plane;
        this.PlaneParent = new GameObject();
        this.PlaneParent.name = "Plane";
        this.Plane.GetComponent<Collider>().tag = "Ground";
    }

    private float getTotalDensity() {
        float density = 0;
        for (int i = 0; i < SpawnTypes.Count; i++) {
            density += SpawnTypes[i].Density();
        }
        return density;
    }
}

public class Chunk {
    private static System.Random rand = new System.Random();
    private float SpawnCount;
    public Rect SpawnArea;

    private List<GameObject> Spawned = new List<GameObject>();

    public Chunk(ChunkTemplate chunkTemplate, Rect newSpawnArea) {
        SpawnArea = newSpawnArea;
        float spawnSurface = SpawnArea.width * SpawnArea.height;
        SpawnCount = spawnSurface * chunkTemplate.TotalDensity;

        SpawnController.spawnPlaneFunc(chunkTemplate, newSpawnArea, Spawned);

        for (int i = 0; i < SpawnCount; i++) {
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(SpawnArea.xMin, SpawnArea.xMax), 0, UnityEngine.Random.Range(SpawnArea.yMin, SpawnArea.yMax));

            // Spawn a random GameObject in the average of the density of all combined GameObjects.
            double randNum = rand.NextDouble() * chunkTemplate.TotalDensity;
            for (int i2 = 0; i2 < chunkTemplate.SpawnTypes.Count; i2++) {
                randNum -= chunkTemplate.SpawnTypes[i2].Density();
                if (randNum < 0) {
                    chunkTemplate.SpawnTypes[i2].SpawnFunc(chunkTemplate.SpawnTypes[i2], randomPosition, Spawned);
                    break;
                }
            }
        }
    }

    public void RemoveChunk() {
        for (int i = Spawned.Count - 1; i >= 0; i--) {
            UnityEngine.Object.DestroyImmediate(Spawned[i]);
            Spawned.RemoveAt(i);
        }
    }
}
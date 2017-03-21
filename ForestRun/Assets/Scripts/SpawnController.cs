using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnController : MonoBehaviour {
    public static void spawnTreeFunc(SpawnableDensityObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(self.Resource.transform.localEulerAngles + new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    public static void spawnFenceFunc(SpawnableDensityObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(Vector3.zero));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    public static void spawnGrassFunc(SpawnableDensityObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    public static void spawnPlaneFunc(Chunk chunk, GameObject plane, Vector3 offset) {
        GameObject obj = Instantiate(plane, offset+new Vector3(chunk.SpawnArea.center.x, 0, chunk.SpawnArea.center.y), Quaternion.identity);
        obj.transform.localScale = new Vector3(chunk.SpawnArea.width / 10, 1, chunk.SpawnArea.height / 10);
        chunk.Spawned.Add(obj);
        obj.transform.parent = chunk.chunkTemplate.PlaneParent.transform;
    }
    public static void spawnCloudFunc(SpawnableDensityObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos + new Vector3(0, UnityEngine.Random.Range(10, 13), 0), Quaternion.identity);
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    public static void spawnItem(Chunk chunk, GameObject item, Vector3 offset) {
        GameObject obj = Instantiate(item, offset + new Vector3(0, 0, chunk.SpawnArea.y), Quaternion.identity);
        chunk.Spawned.Add(obj);
    }
}

public class SpawnableDensityObject {
    public GameObject ParentObject;
    public GameObject Resource;
    public string resourceName;
    public Action<SpawnableDensityObject, Vector3, List<GameObject>> SpawnFunc;

    public SpawnableDensityObject(string newResourceName, Action<SpawnableDensityObject, Vector3, List<GameObject>> newSpawnFunc) {
        this.Resource = (GameObject)Resources.Load(newResourceName);
        this.SpawnFunc = newSpawnFunc;
        this.resourceName = newResourceName;
        ParentObject = new GameObject();
        ParentObject.name = resourceName;
    }
}

public class SpawnableGroup : SpawnableDensityObject {
    public Func<float> Density;

    public SpawnableGroup(string resourceName, Action<SpawnableDensityObject, Vector3, List<GameObject>> newSpawnFunc, Func<float> newDensity) : base(resourceName, newSpawnFunc) {
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
        this.Plane.GetComponent<Collider>().tag = "Ground";
        PlaneParent = new GameObject();
        PlaneParent.name = Plane.name;
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
    private static int seed = (int)DateTime.Now.Ticks;
    private static System.Random rand = new System.Random();
    private float SpawnCount;

    public ChunkTemplate chunkTemplate;
    public Rect SpawnArea;

    public List<GameObject> Spawned = new List<GameObject>();

    public Chunk(ChunkTemplate newChunkTemplate, Rect newSpawnArea) {
        this.chunkTemplate = newChunkTemplate;
        SpawnArea = newSpawnArea;
        float spawnSurface = SpawnArea.width * SpawnArea.height;
        SpawnCount = spawnSurface * chunkTemplate.TotalDensity;
        
        SpawnController.spawnPlaneFunc(this, newChunkTemplate.Plane, new Vector3());

        for (int i = 0; i < SpawnCount; i++) {
            UnityEngine.Random.InitState(seed++);
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(SpawnArea.xMin, SpawnArea.xMax), 0, UnityEngine.Random.Range(SpawnArea.yMin, SpawnArea.yMax));

            this.SpawnRandom(randomPosition);
        }
    }

    public void SpawnRandom(Vector3 pos, string[] bannedList = null) {
        for(int i = 0; i < 100; i++) {
            bool found = false;

            // Spawn a random GameObject in the average of the density of all combined GameObjects.
            double randNum = rand.NextDouble() * chunkTemplate.TotalDensity;
            for (int i2 = 0; i2 < chunkTemplate.SpawnTypes.Count; i2++) {
                randNum -= chunkTemplate.SpawnTypes[i2].Density();
                if (randNum < 0) {
                    if (bannedList == null || !bannedList.Contains(chunkTemplate.SpawnTypes[i2].resourceName)) {
                        chunkTemplate.SpawnTypes[i2].SpawnFunc(chunkTemplate.SpawnTypes[i2], pos, Spawned);
                        found = true;
                        break;
                    }
                }
            }

            if (found) {
                break;
            }
        }
    }

    public List<GameObject> GetSpawned() {
        return Spawned;
    }

    public void RemoveChunk() {
        for (int i = Spawned.Count - 1; i >= 0; i--) {
            UnityEngine.Object.DestroyImmediate(Spawned[i]);
            Spawned.RemoveAt(i);
        }
    }
}
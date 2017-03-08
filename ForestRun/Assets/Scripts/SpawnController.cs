using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {
    public static void spawnTreeFunc(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
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
    public static void spawnPlaneFunc(Chunk chunk, GameObject plane, Vector3 offset) {
        GameObject obj = Instantiate(plane, offset+new Vector3(chunk.SpawnArea.center.x, 0, chunk.SpawnArea.y), Quaternion.identity);
        obj.transform.localScale = new Vector3(chunk.SpawnArea.width / 10, 1, chunk.SpawnArea.height / 10);
        chunk.Spawned.Add(obj);
        obj.transform.parent = chunk.chunkTemplate.PlaneParent.transform;
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
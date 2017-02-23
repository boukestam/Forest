using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {
    private const float ChunkWidthRadius = 80; // X axis
    private const float ChunkLength = 2;  // Z axis

    private const float BeginChunkZ = 10f;
    private const float StartDespawnZ = -10f;
    private const float MinimumRenderDistanceZ = 120f; // The minimum distance (from the player) that the furthest chunk is spawned.

    static void spawnTree(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;

    }
    static void spawnFence(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(Vector3.zero));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    static void spawnGrass(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        GameObject obj = Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0)));
        Spawned.Add(obj);
        obj.transform.parent = self.ParentObject.transform;
    }
    public static void spawnPlane(ChunkTemplate template, Rect spawnArea, List<GameObject> Spawned) {
        GameObject target = Instantiate(template.Plane, new Vector3(spawnArea.center.x, 0, spawnArea.center.y), Quaternion.identity);
        target.transform.localScale = new Vector3(spawnArea.width / 10, 1, spawnArea.height / 10);
        //target.
        Spawned.Add(target);
    }

    Level level1;

    private ChunkTemplate forestChunkTemplate;
    private static float ForestTreeDensity = 0.02f;
    private static float ForestFenceDensity = 0.004f;
    private static float ForestGrassDensity = 0.05f;

    private List<Chunk> chunks = new List<Chunk>();

    private GameObject Player;

    void Start() {
        Player = GameObject.FindWithTag("Player");
        
        forestChunkTemplate = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Tree", spawnTree, () => ForestTreeDensity),
            new SpawnableGroup("Fence", spawnFence, () => ForestFenceDensity),
            new SpawnableGroup("Grass", spawnGrass, () => ForestGrassDensity)
        }, (GameObject)Resources.Load("GrassPlane"));

        level1 = new Level(forestChunkTemplate, 0, 1000, 80);

        RestartSpawns();
    }

    void Update() {
        level1.Update(Player);
    }

    public void RestartSpawns() {
        level1.ResetLevel();
    }
}

public class Level {
    private static float StartDespawnZ = -10;
    private static float MinimumRenderDistanceZ = 120;
    private static float ChunkLength = 2;
    private ChunkTemplate Template;
    private float StartZ;
    private float EndZ;
    private float ChunkWidthRadius;

    private List<Chunk> chunks = new List<Chunk>();

    public Level(ChunkTemplate template, float startZ, float endZ, float chunkWidthRadius) {
        this.Template = template;
        this.StartZ = startZ;
        this.EndZ = endZ;
        this.ChunkWidthRadius = chunkWidthRadius;
    }

    public void Update(GameObject player) {
        // Delete chucks that are out of the screen.
        if (chunks[0].SpawnArea.yMax - player.transform.position.z < StartDespawnZ) {
            chunks[0].RemoveChunk();
            chunks.RemoveAt(0);
        }

        // Add chunks when close enough.
        float lastObjectLocation = chunks[chunks.Count - 1].SpawnArea.yMax;
        if (lastObjectLocation - player.transform.position.z < MinimumRenderDistanceZ) {
            if(EndZ > lastObjectLocation+ ChunkLength) { // Prevent new chunk spawning past the map.
                chunks.Add(new Chunk(Template, new Rect(-ChunkWidthRadius, lastObjectLocation, ChunkWidthRadius * 2, ChunkLength)));
            }
        }
    }

    public void ResetLevel() {
        for (int i = chunks.Count - 1; i >= 0; i--) {
            chunks[i].RemoveChunk();
        }
        chunks.Clear();

        float lastObjectLocation = 0;
        do {
            chunks.Add(new Chunk(this.Template, new Rect(-ChunkWidthRadius, lastObjectLocation, ChunkWidthRadius * 2, ChunkLength)));
            lastObjectLocation = chunks[chunks.Count - 1].SpawnArea.yMax;
        } while (lastObjectLocation - this.StartZ < MinimumRenderDistanceZ);
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

    public ChunkTemplate(List<SpawnableGroup> newSpawnTypes, GameObject plane) {
        SpawnTypes = newSpawnTypes;
        TotalDensity = getTotalDensity();
        this.Plane = plane;
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

        SpawnController.spawnPlane(chunkTemplate, newSpawnArea, Spawned);

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
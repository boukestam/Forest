using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

    public float TreeDensity = 0.02f;
    public float FenceDensity = 0.007f;
    public float GrassDensity = 0.05f;

    private const float ChunkWidthRadius = 150; // X axis
    private const float ChunkLength = 30;  // Z axis

    private const float BeginChunkZ = 10f;
    private const float StartDespawnZ = -10f;
    private const float MinimumRenderDistanceZ = 120f; // The minimum distance (from the player) that the furthest chunk is spawned.

    class SpawnableGameObject {
        public GameObject Resource;
        public Action<SpawnableGameObject, Vector3, List<GameObject>> SpawnFunc;

        public SpawnableGameObject(string resourceName, Action<SpawnableGameObject, Vector3, List<GameObject>> newSpawnFunc) {
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
        Spawned.Add(Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0))));
    }
    static void spawnFence(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        Spawned.Add(Instantiate(self.Resource, pos, Quaternion.Euler(Vector3.zero)));
    }
    static void spawnGrass(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned) {
        Spawned.Add(Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0))));
    }

    private ChunkTemplate testChunkTemplate;
    private List<Chunk> chunks = new List<Chunk>();

    private GameObject Player;

    void Start() {
        Player = GameObject.FindWithTag("Player");

        List<SpawnableGroup> spawnableGroup = new List<SpawnableGroup>();
        spawnableGroup.Add(new SpawnableGroup("Tree", spawnTree, () => TreeDensity));
        spawnableGroup.Add(new SpawnableGroup("Fence", spawnFence, () => FenceDensity));
        spawnableGroup.Add(new SpawnableGroup("Grass", spawnGrass, () => GrassDensity));

        testChunkTemplate = new ChunkTemplate(spawnableGroup);

        InitSpawns();
    }

    void Update() {
        // Delete chucks that are out of the screen.
        if (chunks[0].SpawnArea.yMax - Player.transform.position.z < StartDespawnZ) {
            chunks[0].RemoveChunk();
            chunks.RemoveAt(0);
        }

        float lastObjectLocation = chunks[chunks.Count - 1].SpawnArea.yMax;
        if (lastObjectLocation - Player.transform.position.z < MinimumRenderDistanceZ) {
            chunks.Add(new Chunk(testChunkTemplate, new Rect(Player.transform.position.x - ChunkWidthRadius, lastObjectLocation, ChunkWidthRadius * 2, ChunkLength)));
        }
    }

    public void InitSpawns() {
        for (int i = chunks.Count - 1; i >= 0; i--) {
            chunks[i].RemoveChunk();
        }
        chunks.Clear();

        float lastObjectLocation = BeginChunkZ;
        do {
            chunks.Add(new Chunk(testChunkTemplate, new Rect(Player.transform.position.x - ChunkWidthRadius, lastObjectLocation, ChunkWidthRadius * 2, ChunkLength)));
            lastObjectLocation = chunks[chunks.Count - 1].SpawnArea.yMax;
        } while (lastObjectLocation - Player.transform.position.z < MinimumRenderDistanceZ);
    }
}

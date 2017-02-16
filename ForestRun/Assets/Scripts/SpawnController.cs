using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{

    private const float SpawnRadiusX = 50f;

    private const float RenderDistanceZ = 80f;
    private const float StartDespawnZ = -10f;
    private const float InitSpawnMinZ = 10f;

    class SpawnableGameObject
    {
        public GameObject Resource;
        public float Density;
        public Action<SpawnableGameObject, Vector3, List<GameObject>> SpawnFunc;

        public SpawnableGameObject(float newDensity, string resourceName, Action<SpawnableGameObject, Vector3, List<GameObject>> newSpawnFunc)
        {
            this.Resource = (GameObject)Resources.Load(resourceName);
            this.Density = newDensity;
            this.SpawnFunc = newSpawnFunc;
        }
    }

    private GameObject Player;


    private List<GameObject> Spawned = new List<GameObject>();
    private List<SpawnableGameObject> Spawnable = new List<SpawnableGameObject>();

    // pre-calculated variables
    private const float SpawnLengthZ = RenderDistanceZ - StartDespawnZ;
    private const float InitSpawnMaxZ = InitSpawnMinZ + SpawnLengthZ;
    private const float ContinueSpawnZ = SpawnLengthZ + StartDespawnZ;
    private const float SpawnSurface = SpawnRadiusX * 2 * SpawnLengthZ;
    private float TotalDensity = 0;  // Amount of objects each square meter

    private System.Random rand = new System.Random();

    static void spawnTree(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned)
    {
        Spawned.Add(Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0))));
    }
    static void spawnFence(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned)
    {
        Spawned.Add(Instantiate(self.Resource, pos, Quaternion.Euler(Vector3.zero)));
    }
    static void spawnGrass(SpawnableGameObject self, Vector3 pos, List<GameObject> Spawned)
    {
        Spawned.Add(Instantiate(self.Resource, pos, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0))));
    }

    void SpawnRandom(Vector3 pos)
    {
        double randNum = rand.NextDouble() * TotalDensity;

        for (int i = 0; i < Spawnable.Count; i++)
        {
            randNum -= Spawnable[i].Density;
            if (randNum < 0)
            {
                Spawnable[i].SpawnFunc(Spawnable[i], pos, Spawned);
                break;
            }
        }
    }

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        Spawnable.Add(new SpawnableGameObject(0.03f, "Tree", spawnTree));
        Spawnable.Add(new SpawnableGameObject(0.01f, "Fence", spawnFence));
        Spawnable.Add(new SpawnableGameObject(0.1f, "Grass", spawnGrass));

        for (int i = 0; i < Spawnable.Count; i++)
        {
            TotalDensity += Spawnable[i].Density;
        }
        float GameObjectCount = SpawnSurface * (float)TotalDensity;

        for (int i = 0; i < GameObjectCount; i++)
        {
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-SpawnRadiusX, SpawnRadiusX), 0, UnityEngine.Random.Range(InitSpawnMinZ, InitSpawnMaxZ));
            SpawnRandom(randomPosition);
        }
        Spawned.Sort((obj1, obj2) => obj1.transform.position.z.CompareTo(obj2.transform.position.z));
    }

    void Update()
    {
        for (int i = 0; i < Spawned.Count; i++)
        {
            if (Spawned[i].transform.position.z >= Player.transform.position.z + StartDespawnZ)
            {
                break;
            }
            Destroy(Spawned[i]);
            Spawned.RemoveAt(i);

            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(Player.transform.position.x - SpawnRadiusX, Player.transform.position.x + SpawnRadiusX),
                0,
                UnityEngine.Random.Range(Player.transform.position.z + ContinueSpawnZ, Player.transform.position.z + ContinueSpawnZ)
            );

            SpawnRandom(randomPosition);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

    private GameObject Tree;
    private GameObject Fence;
    private GameObject Grass;

    private GameObject Player;

    private List<GameObject> Spawned = new List<GameObject>();

    private const float RenderDistance = 30f;

    private const float SpawnRadiusX = 50f;
    private const float StartDespawnZ = -10f;
    private const float SpawnLengthZ = RenderDistance - StartDespawnZ;

    private const float SpawnSurface = SpawnRadiusX * 2 * SpawnLengthZ;
    private const double TreeDensity = 0.02f; // Amount of objects each square meter
    private const double FenceDensity = 0.01f; // Amount of objects each square meter
    private const double GrassDensity = 0.1f; // Amount of objects each square meter
    private const double TotalDensity = TreeDensity + FenceDensity + GrassDensity;
    private const float GameObjectCount = SpawnSurface * (float)TotalDensity;

    private const float InitSpawnMinZ = 10f;
    private const float InitSpawnMaxZ = InitSpawnMinZ + SpawnLengthZ;

    private const float ContinueSpawnZ = SpawnLengthZ + StartDespawnZ;

    private System.Random rand = new System.Random();

    void Spawn(Vector3 pos) {
        double randNum = rand.NextDouble() * TotalDensity;

        GameObject obj;

        if ((randNum -= TreeDensity) < 0)
        {
            // Tree

            obj = Instantiate(Tree, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        }
        else if ((randNum -= FenceDensity) < 0)
        {
            // Fence
            obj = Instantiate(Fence, pos, Quaternion.Euler(Vector3.zero));
        }
        else if((randNum -= GrassDensity) < 0)
        {
            
            // Grass
            obj = Instantiate(Grass, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        }
        else
        {
            Debug.Log("Impossible???");
            // Default
            obj = Instantiate(Grass, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        }

        Spawned.Add(obj);
    }

    void Start()
    {
        Player = GameObject.FindWithTag("Player");

        Tree = (GameObject)Resources.Load("Tree");
        Fence = (GameObject)Resources.Load("Fence");
        Grass = (GameObject)Resources.Load("Grass");

        for (int i = 0; i < GameObjectCount; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-SpawnRadiusX, SpawnRadiusX), 0, Random.Range(InitSpawnMinZ, InitSpawnMaxZ));

            Spawn(randomPosition);
        }
        Spawned.Sort((obj1, obj2) => obj1.transform.position.z.CompareTo(obj2.transform.position.z));
    }

    void Update () {
        for(int i = 0; i < Spawned.Count; i++) {
            if(Spawned[i].transform.position.z >= Player.transform.position.z + StartDespawnZ) {
                break;
            }
            Destroy(Spawned[i]);
            Spawned.RemoveAt(i);

            Vector3 randomPosition = new Vector3(
                Random.Range(Player.transform.position.x - SpawnRadiusX, Player.transform.position.x + SpawnRadiusX),
                0,
                Random.Range(Player.transform.position.z + ContinueSpawnZ, Player.transform.position.z + ContinueSpawnZ)
            );

            Spawn(randomPosition);
        }
    }
}

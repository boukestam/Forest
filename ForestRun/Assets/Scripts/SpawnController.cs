using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

    private GameObject Tree;
    private GameObject Fence;
    private GameObject Grass;

    private GameObject Player;

    private List<GameObject> Spawned = new List<GameObject>();

    void Spawn(Vector3 pos) {
        int rand = Random.Range(0, 10);

        GameObject obj;

        if (rand < 5) {
            // Tree

            obj = Instantiate(Tree, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        } else if(rand < 6) {
            // Fence

            obj = Instantiate(Fence, pos, Quaternion.Euler(Vector3.zero));
        } else {
            // Grass

            obj = Instantiate(Grass, pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        }

         
        Spawned.Add(obj);
    }

    private const float SpawnRadiusX = 50f;
    private const float SpawnLengthZ = 40f;
    private const float InitSpawnMinZ = 10f;
    private const float InitSpawnMaxZ = InitSpawnMinZ + SpawnLengthZ;
    private const float StartDespawnZ = -10f;
    private const float ContinueSpawnMinZ = InitSpawnMaxZ + StartDespawnZ;
    private const float ContinueSpawnMaxZ = ContinueSpawnMinZ + SpawnLengthZ;

    void Start () {
        Player = GameObject.FindWithTag("Player");

        Tree = (GameObject)Resources.Load("Tree");
        Fence = (GameObject)Resources.Load("Fence");
        Grass = (GameObject)Resources.Load("Grass");

        for (int i = 0; i < 300; i++) {
            Vector3 randomPosition = new Vector3(Random.Range(-SpawnRadiusX, SpawnRadiusX), 0, Random.Range(InitSpawnMinZ, InitSpawnMaxZ));

            Spawn(randomPosition);
        }
    }
	
	void Update () {
        for(int i = 0; i < Spawned.Count; i++) {
            if(Spawned[i].transform.position.z < Player.transform.position.z + StartDespawnZ) {
                Destroy(Spawned[i]);
                Spawned.RemoveAt(i);

                Vector3 randomPosition = new Vector3(
                    Random.Range(Player.transform.position.x - SpawnRadiusX, Player.transform.position.x + SpawnRadiusX),
                    0,
                    Random.Range(Player.transform.position.z + ContinueSpawnMinZ, Player.transform.position.z + ContinueSpawnMaxZ)
                );

                Spawn(randomPosition);
            }
        }
    }
}

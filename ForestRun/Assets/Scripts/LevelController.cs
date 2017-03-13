using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    private LevelManager levelManager;

    void Start() {
        levelManager = new LevelManager();
        LoadLevels();

        RestartCurrentLevel();
    }

    void Update() {
        levelManager.Update();
    }

    public static void LoadLevels()
    {
        const float forestTreeDensity = 0.01f;
        const float forestFenceDensity = 0.004f;
        const float forestGrassDensity = 0.05f;
        ChunkTemplate forestChunkTemplate = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Tree", SpawnController.spawnTreeFunc, () => forestTreeDensity),
            new SpawnableGroup("Fence", SpawnController.spawnFenceFunc, () => forestFenceDensity),
            new SpawnableGroup("Grass", SpawnController.spawnGrassFunc, () => forestGrassDensity)
        }, (GameObject)Resources.Load("GrassPlane"));

        const float forestTreeDensity2 = 0.03f;
        const float forestFenceDensity2 = 0.008f;
        const float forestGrassDensity2 = 0.05f;
        ChunkTemplate forestChunkTemplate2 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Tree", SpawnController.spawnTreeFunc, () => forestTreeDensity2),
            new SpawnableGroup("Fence", SpawnController.spawnFenceFunc, () => forestFenceDensity2),
            new SpawnableGroup("Grass", SpawnController.spawnGrassFunc, () => forestGrassDensity2)
        }, (GameObject)Resources.Load("GrassPlane"));

        LevelManager.levels = new List<Level>();
        LevelManager.levels.Add(new Level(1, forestChunkTemplate, 0, 100, 80, true));
        LevelManager.levels.Add(new Level(2, forestChunkTemplate2, 100, 1000, 80));

        /*levelManager = new LevelManager(new List<Level>() {
            new Level(1, forestChunkTemplate, 0, 100, 80, true),
            new Level(2, forestChunkTemplate2, 100, 1000, 80)
        });*/

    }

    public void RestartCurrentLevel() {
        levelManager.RestartCurrentLevel();
    }
}
[System.Serializable]
public class LevelManager {
    public static List<Level> levels;
    private int currentLevel;
    private GameObject scorePanel;
    private PlayerController playerController;
    bool scoreMenu = false;

    public LevelManager() {
        scorePanel = GameObject.Find("ScorePanel");
        scorePanel.SetActive(false);
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        currentLevel = PlayerPrefs.GetInt("lastPlayedLevel") - 1;
    }

    private void EnterScorePanel() {
        if (playerController.getPoints() > PlayerPrefs.GetInt("Level" + (currentLevel + 1) + "_score"))
        {
            PlayerPrefs.SetInt("Level" + (currentLevel + 1) + "_score", playerController.getPoints());
        }
        scoreMenu = true;
        scorePanel.SetActive(true);
        scorePanel.transform.FindChild("Score").gameObject.GetComponent<Text>().text = "Score: "+ playerController.getPoints();
        playerController.Freeze();
    }

    private void ExitScorePanel() {
        scoreMenu = false;
        scorePanel.SetActive(false);
        playerController.Unfreeze();
    }

    private void NextLevel()
    {
        playerController.setPoints(0);
        ExitScorePanel();
        levels[currentLevel].ClearLevel();
        currentLevel++;
        //Unlock new level and save this level as last level
        PlayerPrefs.SetInt("lastPlayedLevel", currentLevel + 1);
        PlayerPrefs.SetInt("Level" + (currentLevel + 1), 1);
        if (currentLevel >= levels.Count) {
            currentLevel = levels.Count - 1;
        }
    }

    public void Update() {
        if (scoreMenu) {
            if (Input.GetButtonDown("Submit")) {
                NextLevel();
            }
        } else {
            // Check for going to new level.
            if (levels[currentLevel].completedLevel()) {
                EnterScorePanel();
            }

            levels[currentLevel].Update();
            if (currentLevel + 1 < levels.Count) {
                levels[currentLevel + 1].Update();
            }
        }
    }

    public void RestartCurrentLevel() {
        levels[currentLevel].ResetLevel();
        if (currentLevel + 1 < levels.Count) {
            levels[currentLevel + 1].ClearLevel();
        }
        GameObject.FindWithTag("Player").transform.position = new Vector3(0, 0, levels[currentLevel].StartZ);
    }
}

[System.Serializable]
public class Level {
    public int levelNumber;
    public bool unlocked;
    public bool isInteractable;

    private GameObject Player;
    private static float StartDespawnZ = -10;
    private static float MinimumRenderDistanceZ = 80;
    private static float ChunkLength = 2;
    private ChunkTemplate Template;
    public float StartZ;
    private float EndZ;
    private float ChunkWidthRadius;

    private List<Chunk> chunks = new List<Chunk>();

    public Level(int number, ChunkTemplate template, float startZ, float endZ, float chunkWidthRadius, bool unlocked = false) {
        Player = GameObject.FindWithTag("Player");
        this.levelNumber = number;
        this.Template = template;
        this.StartZ = startZ;
        this.EndZ = endZ;
        this.ChunkWidthRadius = chunkWidthRadius;
        this.unlocked = unlocked;
        this.isInteractable = unlocked;

        /*// Temporary faking a straight item path
        float length = 10;
        for (int i = 1; i < 20; i++) {
            itemPath.Add(new Vector3(0,0, length*i));
        }*/
    }

    public void Update() {
        // Delete chucks that are out of the screen.
        if (chunks.Count > 0 && chunks[0].SpawnArea.yMax - Player.transform.position.z < StartDespawnZ) {
            removeChunk(0);
        }

        // Add chunks when close enough to the edge of all chunks.
        float furdestLocationZ = (chunks.Count > 0) ? chunks[chunks.Count - 1].SpawnArea.yMax : this.StartZ;
        if (furdestLocationZ - Player.transform.position.z < MinimumRenderDistanceZ) {
            SpawnChunk(furdestLocationZ);
        }
    }

    public bool completedLevel() {
        return Player.transform.position.z + ChunkLength >= this.EndZ;
    }

    public void ResetLevel() {
        ClearLevel();

        float furdestLocationZ = this.StartZ;
        do {
            SpawnChunk(furdestLocationZ);
            furdestLocationZ = chunks[chunks.Count - 1].SpawnArea.yMax;
        } while (furdestLocationZ - this.StartZ < MinimumRenderDistanceZ);
    }

    public void ClearLevel() {
        for (int i = chunks.Count - 1; i >= 0; i--) {
            this.removeChunk(i);
        }
    }

    private void removeChunk(int index) {
        chunks[index].RemoveChunk();
        chunks.RemoveAt(index);
    }
    
    private List<Vector3> GetPath(float startZ, float endZ, float pathLength, float pathWidth, float pathStepSize) {
        List<Vector3> path = new List<Vector3>();

        for (float z = this.StartZ; z < this.EndZ; z += pathStepSize) {
            Vector3 pathPoint = new Vector3(Mathf.Sin(z / pathLength) * pathWidth, 0, z);
            path.Add(pathPoint);
        }

        return path;
    }

    private void SpawnChunk(float chunkStartZ) {
        // For testing
        GameObject pathBlueprint = (GameObject)Resources.Load("Path");

        if (chunkStartZ + ChunkLength <= this.EndZ) { // Prevent new chunk spawning past the map.
            if (chunkStartZ >= this.StartZ) { // Prevent new chunk spawning before the map.
                Chunk newChunk = new Chunk(Template, new Rect(-ChunkWidthRadius, chunkStartZ, ChunkWidthRadius * 2, ChunkLength));

                // Clear path in chunk

                List<Vector3> path = GetPath(this.StartZ, this.EndZ, 20, 10, 1);

                float clearRadius = 2;

                List<GameObject> spawned = newChunk.GetSpawned();

                for(int i = 0; i < spawned.Count; i++) {
                    GameObject obj = spawned[i];

                    if (obj.tag != "Ground") {
                        foreach (Vector3 pathPoint in path) {
                            if ((obj.transform.position - pathPoint).magnitude < clearRadius) {
                                SpawnController.Destroy(obj);
                                spawned.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }

                foreach (Vector3 pathPoint in path) {
                    if(newChunk.SpawnArea.Contains(new Vector2(pathPoint.x, pathPoint.z))) {
                        spawned.Add(SpawnController.Instantiate(pathBlueprint, new Vector3(pathPoint.x, 0.1f, pathPoint.z), new Quaternion()));
                    }
                }

                // Add coins to the chunk.
                //float chunkStartRelativeZ = chunkStartZ - this.StartZ;
                for (int i = 0; i < path.Count; i++) {
                    Vector3 boneLocation = path[i];
                    if (boneLocation.z > chunkStartZ && boneLocation.z <= chunkStartZ + ChunkLength) {
                        if (Random.Range(0.0f, 1.0f) > 0.9f) {
                            boneLocation.z -= chunkStartZ;
                            if (Random.Range(0.0f, 1.0f) >= 0.5f) {
                                boneLocation.x += Random.Range(1, 2);
                            }
                            SpawnController.spawnItem(newChunk, (GameObject)Resources.Load("BoneItem"), boneLocation);
                        }
                    }

                }

                // If last chunk add finish plane.
                if (chunkStartZ + ChunkLength == this.EndZ) {
                    SpawnController.spawnPlaneFunc(newChunk, (GameObject)Resources.Load("FinishPlane"), new Vector3(0, 0.001f, 0));
                }

                chunks.Add(newChunk);
            }
        }
    }
}
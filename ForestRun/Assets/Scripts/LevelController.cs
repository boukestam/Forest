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
        const float snowTreeDensity1 = 0.0008f;
        const float snowFenceDensity1 = 0.01f;
        ChunkTemplate snowChunkTemplate1 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Pinetree", SpawnController.spawnTreeFunc, () => snowTreeDensity1),
            new SpawnableGroup("PenguinGroup", SpawnController.spawnFenceFunc, () => snowFenceDensity1)
        }, (GameObject)Resources.Load("PlaneSnow"));

        const float snowTreeDensity2 = 0.02f;
        const float snowFenceDensity2 = 0.0012f;
        ChunkTemplate snowChunkTemplate2 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Pinetree", SpawnController.spawnTreeFunc, () => snowTreeDensity2),
            new SpawnableGroup("PenguinGroup", SpawnController.spawnFenceFunc, () => snowFenceDensity2)
        }, (GameObject)Resources.Load("PlaneSnow"));

        const float forestTreeDensity1 = 0.01f;
        const float forestFenceDensity1 = 0.004f;
        const float forestGrassDensity1 = 0.05f;
        ChunkTemplate forestChunkTemplate1 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Tree", SpawnController.spawnTreeFunc, () => forestTreeDensity1),
            new SpawnableGroup("Fence", SpawnController.spawnFenceFunc, () => forestFenceDensity1),
            new SpawnableGroup("Grass", SpawnController.spawnGrassFunc, () => forestGrassDensity1)
        }, (GameObject)Resources.Load("PlaneGrass"));

        const float forestTreeDensity2 = 0.03f;
        const float forestFenceDensity2 = 0.008f;
        const float forestGrassDensity2 = 0.05f;
        ChunkTemplate forestChunkTemplate2 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Tree", SpawnController.spawnTreeFunc, () => forestTreeDensity2),
            new SpawnableGroup("Fence", SpawnController.spawnFenceFunc, () => forestFenceDensity2),
            new SpawnableGroup("Grass", SpawnController.spawnGrassFunc, () => forestGrassDensity2)
        }, (GameObject)Resources.Load("PlaneGrass"));

        int amountOfBones = 10;
        int levelLength = 100;
        float levelWidth = 30;
        LevelManager.levels = new List<Level>();
        Level level1 = new Level(1, snowChunkTemplate1, (GameObject)Resources.Load("Mountain"), 0, levelLength, levelWidth, amountOfBones, true);
        Level level2 = new Level(2, snowChunkTemplate2, (GameObject)Resources.Load("Mountain"), level1.EndZ, level1.EndZ + levelLength, levelWidth, amountOfBones);
        Level level3 = new Level(3, forestChunkTemplate1, (GameObject)Resources.Load("Mountain"), level2.EndZ, level2.EndZ + levelLength, levelWidth, amountOfBones);
        Level level4 = new Level(4, forestChunkTemplate2, (GameObject)Resources.Load("Mountain"), level3.EndZ, level3.EndZ + levelLength, levelWidth, amountOfBones);
        LevelManager.levels.Add(level1);
        LevelManager.levels.Add(level2);
        LevelManager.levels.Add(level3);
        LevelManager.levels.Add(level4);
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
        if (playerController.getPoints() > PlayerPrefs.GetInt("Level" + (currentLevel + 1) + "_score")) {
            PlayerPrefs.SetInt("Level" + (currentLevel + 1) + "_score", playerController.getPoints());
        }
        scoreMenu = true;
        scorePanel.SetActive(true);
        scorePanel.transform.FindChild("Score").gameObject.GetComponent<Text>().text = "Score: " + playerController.getPoints();
        playerController.Freeze();
    }

    private void ExitScorePanel() {
        scoreMenu = false;
        scorePanel.SetActive(false);
        playerController.Unfreeze();
    }

    private void NextLevel() {
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
        playerController.gameObject.transform.position = new Vector3(0, playerController.gameObject.transform.position.y, playerController.gameObject.transform.position.z);
        playerController.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void Update() {
        if (scoreMenu) {
            if (Input.GetButtonDown("Jump")) {
                NextLevel();
            }
        } else {
            // Check for going to new level.
            if (levels[currentLevel].CompletedLevel()) {
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
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = new Vector3(0, 0, levels[currentLevel].StartZ);
        player.transform.eulerAngles = new Vector3(0, 0, 0);
    }
}

[System.Serializable]
public class Level {
    public int levelNumber;
    public bool unlocked;
    public bool isInteractable;

    private static float StartDespawnZ = -10;
    private static float MinimumRenderDistanceZ = 80;
    private static float ChunkLength = 2;

    private GameObject Player;
    private ChunkTemplate Template;
    private GameObject edgePrefab;
    public float furdestPlacedEdge = 0;
    public float StartZ;
    public float amountOfBones;
    public float EndZ;
    private float ChunkWidthRadius;

    GameObject rememberEdgeLeft = null;
    GameObject rememberEdgeRight = null;

    private List<Chunk> chunks = new List<Chunk>();
    List<Vector3> path = new List<Vector3>();
    List<Vector3> bones = new List<Vector3>();

    private int Seed;

    public Level(int number, ChunkTemplate template, GameObject newEdgePrefab, float startZ, float endZ, float chunkWidthRadius, int newAmountOfBones, bool unlocked = false) {
        Player = GameObject.FindWithTag("Player");
        this.levelNumber = number;
        this.Template = template;
        this.edgePrefab = newEdgePrefab;
        this.StartZ = startZ;
        this.EndZ = endZ;
        this.ChunkWidthRadius = chunkWidthRadius;
        this.amountOfBones = newAmountOfBones;
        this.unlocked = unlocked;
        this.isInteractable = unlocked;

        /*// Temporary faking a straight item path
        float length = 10;
        for (int i = 1; i < 20; i++) {
            itemPath.Add(new Vector3(0,0, length*i));
        }*/
        this.Seed = Random.Range(0, 1000000);
        this.furdestPlacedEdge = StartZ;

        path = GetPath(this.StartZ, this.EndZ, 0.3f, 1);
        GenerateBoneLocations(path);
    }

    private void GenerateBoneLocations(List<Vector3> newPath) {
        bones.Clear();
        Random.InitState(System.DateTime.Now.Millisecond);

        float stepSize = (this.EndZ - this.StartZ) / this.amountOfBones;

        // Tweakable variables
        float percentageChangeOffPath = 0.5f;
        float offPathMinimumX = 1.0f;
        float offPathMaximumX = 2.0f;
        float maxRandomDisplacementZ = stepSize / 3.0f;

        float lastBoneLocationZ = this.StartZ - stepSize;
        for (int i = 0; i < newPath.Count; i++) {
            if (newPath[i].z > this.EndZ) {
                break;
            }
            if (newPath[i].z > lastBoneLocationZ + stepSize) {
                lastBoneLocationZ += stepSize;
                Vector3 boneLocation = new Vector3(newPath[i].x, newPath[i].y, newPath[i].z);
                float negativeRange = (bones.Count > 0) ? maxRandomDisplacementZ : -maxRandomDisplacementZ / 1.5f;
                float possitiveRange = (bones.Count < this.amountOfBones - 2) ? maxRandomDisplacementZ : -maxRandomDisplacementZ / 1.5f;
                boneLocation.z += Random.Range(-negativeRange, possitiveRange);
                if (Random.Range(0.0f, 1.0f) < percentageChangeOffPath) {
                    boneLocation.x += Random.Range(0.0f, 1.0f) >= 0.5f ? Random.Range(-offPathMinimumX, -offPathMaximumX) : Random.Range(offPathMinimumX, offPathMaximumX);
                }
                bones.Add(boneLocation);
            }
        }
    }

    public void Update() {
        // Delete chucks that are out of the screen.
        if (chunks.Count > 0 && chunks[0].SpawnArea.yMax - Player.transform.position.z < StartDespawnZ) {
            RemoveChunk(0);
        }

        // Add chunks when close enough to the edge of all chunks.
        float furdestLocationZ = (chunks.Count > 0) ? chunks[chunks.Count - 1].SpawnArea.yMax : this.StartZ;
        if (furdestLocationZ - Player.transform.position.z < MinimumRenderDistanceZ) {
            SpawnChunk(furdestLocationZ);
        }
    }

    public bool CompletedLevel() {
        return Player.transform.position.z >= this.EndZ;
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
            this.RemoveChunk(i);
        }
        SpawnController.Destroy(rememberEdgeLeft);
        SpawnController.Destroy(rememberEdgeRight);
        this.furdestPlacedEdge = StartZ;
        path = GetPath(this.StartZ, this.EndZ, 0.3f, 1);
        GenerateBoneLocations(path);
    }

    private void RemoveChunk(int index) {
        chunks[index].RemoveChunk();
        chunks.RemoveAt(index);
    }

    private List<Vector3> GetPath(float startZ, float endZ, float randomness, float pathStepSize) {
        List<Vector3> path = new List<Vector3>();
        float x = 0;
        float deltaX = 0;

        float maxDeltaX = 0.5f;

        Random.InitState(this.Seed + (int)(startZ * 100));

        for (float z = startZ; z < endZ; z += pathStepSize) {
            Vector3 pathPoint = new Vector3(x, 0, z);
            path.Add(pathPoint);

            deltaX += Random.Range(-randomness, randomness);
            if (deltaX > maxDeltaX) {
                deltaX = maxDeltaX;
            }
            if (deltaX < -maxDeltaX) {
                deltaX = -maxDeltaX;
            }

            if(Mathf.Abs(x + (deltaX * 10)) > this.ChunkWidthRadius) {
                deltaX *= 0.5f;
            }

            x += deltaX;
        }

        return path;
    }

    private void SpawnChunk(float chunkStartZ) {
        // For testing
        GameObject pathBlueprint = (GameObject)Resources.Load("Path");

        if (chunkStartZ + ChunkLength <= this.EndZ) { // Prevent new chunk spawning past the map.
            if (chunkStartZ >= this.StartZ) { // Prevent new chunk spawning before the map.
                Chunk newChunk = new Chunk(Template, new Rect(-ChunkWidthRadius, chunkStartZ, ChunkWidthRadius * 2, ChunkLength));

                float clearRadius = 2;

                List<GameObject> spawned = newChunk.GetSpawned();

                for (int i = 0; i < spawned.Count; i++) {
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

                /*
                foreach (Vector3 pathPoint in path) {
                    if (newChunk.SpawnArea.Contains(new Vector2(pathPoint.x, pathPoint.z))) {
                        spawned.Add(SpawnController.Instantiate(pathBlueprint, new Vector3(pathPoint.x, 0.01f, pathPoint.z), new Quaternion()));
                    }
                }
                */

                // Add bones to the chunk.
                Random.InitState(System.DateTime.Now.Millisecond);
                for (int i = 0; i < bones.Count; i++) {
                    Vector3 boneLocation = new Vector3(bones[i].x, bones[i].y, bones[i].z);
                    if (boneLocation.z >= chunkStartZ && boneLocation.z <= chunkStartZ + ChunkLength) {
                        boneLocation.z -= chunkStartZ;
                        SpawnController.spawnItem(newChunk, (GameObject)Resources.Load("BoneItem"), boneLocation);
                    }

                }

                // If last chunk add finish plane.
                if (chunkStartZ + ChunkLength == this.EndZ) {
                    SpawnController.spawnPlaneFunc(newChunk, (GameObject)Resources.Load("FinishPlane"), new Vector3(0, 0.001f, 0));
                }

                // Add edge objects to the map.
                if (furdestPlacedEdge < chunkStartZ) {
                    // Add edges to the the chunk in just passed the last location of the edge.
                    if (rememberEdgeLeft != null && rememberEdgeRight != null) {
                        newChunk.Spawned.Add(rememberEdgeLeft);
                        newChunk.Spawned.Add(rememberEdgeRight);
                    }
                    float edgeWidthRadius = edgePrefab.GetComponent<Renderer>().bounds.size.x / 2;
                    float edgeLength = edgePrefab.GetComponent<Renderer>().bounds.size.z;
                    float offsetZ = edgeLength / 2;
                    rememberEdgeLeft = SpawnController.Instantiate(edgePrefab, new Vector3(-ChunkWidthRadius - edgeWidthRadius, 0, furdestPlacedEdge + offsetZ), Quaternion.identity);
                    rememberEdgeRight = SpawnController.Instantiate(edgePrefab, new Vector3(ChunkWidthRadius + edgeWidthRadius, 0, furdestPlacedEdge + offsetZ), Quaternion.identity);
                    furdestPlacedEdge += edgeLength;
                }

                chunks.Add(newChunk);
            }
        }
    }
}
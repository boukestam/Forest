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

    public static void LoadLevels() {
        const float cloudDensity = 0.004f;

        ChunkTemplate snowChunkTemplate1 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Pinetree", SpawnController.spawnTreeFunc, () => 0.0008f),
            new SpawnableGroup("PenguinGroup", SpawnController.spawnFenceFunc, () => 0.01f)
        }, (GameObject)Resources.Load("PlaneSnow"));

        ChunkTemplate snowChunkTemplate2 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Pinetree", SpawnController.spawnTreeFunc, () => 0.015f),
            new SpawnableGroup("PenguinGroup", SpawnController.spawnFenceFunc, () => 0.0012f)
        }, (GameObject)Resources.Load("PlaneSnow"));

        ChunkTemplate snowChunkTemplate3 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Pinetree", SpawnController.spawnTreeFunc, () => 0.018f),
            new SpawnableGroup("PenguinGroup", SpawnController.spawnFenceFunc, () => 0.012f)
        }, (GameObject)Resources.Load("PlaneSnow"));

        ChunkTemplate forestChunkTemplate1 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Tree", SpawnController.spawnTreeFunc, () => 0.006f),
            new SpawnableGroup("Fence", SpawnController.spawnFenceFunc, () => 0.01f),
            new SpawnableGroup("Grass", SpawnController.spawnGrassFunc, () => 0.05f)
        }, (GameObject)Resources.Load("PlaneGrass"));

        ChunkTemplate forestChunkTemplate2 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Tree", SpawnController.spawnTreeFunc, () => 0.03f),
            new SpawnableGroup("Fence", SpawnController.spawnFenceFunc, () => 0.01f),
            new SpawnableGroup("Grass", SpawnController.spawnGrassFunc, () => 0.05f)
        }, (GameObject)Resources.Load("PlaneGrass"));

        ChunkTemplate forestChunkTemplate3 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Tree", SpawnController.spawnTreeFunc, () => 0.03f),
            new SpawnableGroup("Fence", SpawnController.spawnFenceFunc, () => 0.03f),
            new SpawnableGroup("Grass", SpawnController.spawnGrassFunc, () => 0.05f)
        }, (GameObject)Resources.Load("PlaneGrass"));

        ChunkTemplate cityChunkTemplate1 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Trash", SpawnController.spawnTreeFunc, () => 0.03f),
            new SpawnableGroup("Lamppost", SpawnController.spawnFenceFunc, () => 0.015f)
        }, (GameObject)Resources.Load("PlaneAsphalt"));

        ChunkTemplate cityChunkTemplate2 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Trash", SpawnController.spawnTreeFunc, () => 0.01f),
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Car", SpawnController.spawnTreeFunc, () => 0.015f),
            new SpawnableGroup("Lamppost", SpawnController.spawnFenceFunc, () => 0.015f)
        }, (GameObject)Resources.Load("PlaneAsphalt"));

        ChunkTemplate cityChunkTemplate3 = new ChunkTemplate(new List<SpawnableGroup>() {
            new SpawnableGroup("Trash", SpawnController.spawnTreeFunc, () => 0.01f),
            new SpawnableGroup("Cloud", SpawnController.spawnCloudFunc, () => cloudDensity),
            new SpawnableGroup("Car", SpawnController.spawnTreeFunc, () => 0.02f),
            new SpawnableGroup("Lamppost", SpawnController.spawnFenceFunc, () => 0.015f)
        }, (GameObject)Resources.Load("PlaneAsphalt"));

        GameObject walls = (GameObject)Resources.Load("Mountain");
        GameObject building = (GameObject)Resources.Load("Building");
        int amountOfBones = 20;
        int levelLength = 400;
        float levelWidth = 30;
        List<Level> lvls = new List<Level>();

        lvls.Add(new Level(lvls.Count + 1, snowChunkTemplate1, walls, 0, levelLength, levelWidth, amountOfBones, true));
        lvls.Add(new Level(lvls.Count + 1, forestChunkTemplate1, walls, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));
        lvls.Add(new Level(lvls.Count + 1, cityChunkTemplate1, building, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));

        lvls.Add(new Level(lvls.Count + 1, snowChunkTemplate2, walls, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));
        lvls.Add(new Level(lvls.Count + 1, forestChunkTemplate2, walls, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));
        lvls.Add(new Level(lvls.Count + 1, cityChunkTemplate2, building, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));

        lvls.Add(new Level(lvls.Count + 1, snowChunkTemplate3, walls, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));
        lvls.Add(new Level(lvls.Count + 1, forestChunkTemplate3, walls, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));
        lvls.Add(new Level(lvls.Count + 1, cityChunkTemplate3, building, lvls[lvls.Count - 1].EndZ, lvls[lvls.Count - 1].EndZ + levelLength, levelWidth, amountOfBones));
        lvls[lvls.Count - 1].SetLastLevel(true);
        LevelManager.levels = lvls;
    }

    public void RestartCurrentLevel() {
        ((PlayerController)GameObject.Find("Player").GetComponent("PlayerController")).Restart();

        levelManager.RestartCurrentLevel();
    }

    public void NextLevel() {
        levelManager.NextLevel();
    }
}

public class LevelManager {
    public static List<Level> levels;
    private int currentLevel;
    private GameObject scorePanel;
    private GameObject star1;
    private GameObject star2;
    private GameObject star3;
    private PlayerController playerController;
    bool scoreMenu = false;

    public LevelManager() {
        scorePanel = GameObject.Find("ScorePanel");

        star1 = GameObject.Find("Star1");
        star2 = GameObject.Find("Star2");
        star3 = GameObject.Find("Star3");

        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);

        scorePanel.SetActive(false);
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        currentLevel = PlayerPrefs.GetInt("lastPlayedLevel") - 1;
    }

    private void EnterScorePanel() {
        if (playerController.getPoints() > PlayerPrefs.GetInt("Level" + (currentLevel + 1) + "_score")) {
            PlayerPrefs.SetInt("Level" + (currentLevel + 1) + "_score", playerController.getPoints());
        }
        PlayerPrefs.SetInt("lastPlayedLevel", currentLevel + 2);
        PlayerPrefs.SetInt("Level" + (currentLevel + 2), 1);
        scoreMenu = true;
        scorePanel.SetActive(true);
        int score = playerController.getPoints();
        switch (Level.GetStars(score, (int)levels[currentLevel].amountOfBones)) {
            case 1:
                star1.SetActive(true);
                break;
            case 2:
                star1.SetActive(true);
                star2.SetActive(true);
                break;
            case 3:
                star1.SetActive(true);
                star2.SetActive(true);
                star3.SetActive(true);
                break;
        }
        scorePanel.transform.FindChild("Score").gameObject.GetComponent<Text>().text = "Score: " + score;
        playerController.Freeze();
    }

    private void ExitScorePanel() {
        scoreMenu = false;
        scorePanel.SetActive(false);
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
        playerController.Unfreeze();
    }

    public void NextLevel() {
        if (currentLevel >= levels.Count - 1) {
            GameObject.Find("Controller").GetComponent<MenuController>().OnMainMenu();
            return;
        }
        playerController.setPoints(0);
        ExitScorePanel();
        levels[currentLevel].ClearLevel();
        currentLevel++;

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
                levels[currentLevel].Update();
            }
        }
    }

    public void RestartCurrentLevel() {
        ExitScorePanel();
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
    public ChunkTemplate Template;
    private GameObject edgePrefab;
    public float furdestPlacedEdge = 0;
    public float StartZ;
    public float amountOfBones;
    public float EndZ;
    private float ChunkWidthRadius;

    private bool lastLevel = false;

    GameObject rememberEdgeLeft = null;
    GameObject rememberEdgeRight = null;

    private List<Chunk> chunks = new List<Chunk>();
    List<Vector3> path = new List<Vector3>();
    List<Vector3> bones = new List<Vector3>();
    private int spawnedBoneLocation = 0;

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

    public static int GetStars(int score, int maxScore) {
        if (score >= maxScore) {
            return 3;
        }
        if (score > maxScore / 2) {
            return 2;
        }
        if (score > maxScore / 10) {
            return 1;
        }
        return 0;
    }

    private void GenerateBoneLocations(List<Vector3> newPath) {
        bones.Clear();
        Random.InitState(System.DateTime.Now.Millisecond);

        float preStepSize = (this.EndZ - this.StartZ) / this.amountOfBones;
        float offsetStartBoneSpawnZ = preStepSize / 2.0f;
        float stepSize = (this.EndZ - this.StartZ - offsetStartBoneSpawnZ) / this.amountOfBones;

        // Tweakable variables
        float percentageChangeOffPath = 0.5f;
        float offPathMinimumX = 0.0f;
        float offPathMaximumX = 0.2f;
        float maxRandomDisplacementZ = stepSize / 3.0f;

        float randomAdder = offsetStartBoneSpawnZ + Random.Range(maxRandomDisplacementZ / 1.5f, maxRandomDisplacementZ);
        float lastBoneLocationZ = this.StartZ - stepSize + offsetStartBoneSpawnZ;
        for (int i = 0; i < newPath.Count; i++) {
            if (newPath[i].z > this.EndZ) {
                break;
            }
            if (newPath[i].z > lastBoneLocationZ + stepSize + randomAdder) {
                lastBoneLocationZ += stepSize;
                Vector3 boneLocation = new Vector3(newPath[i].x, newPath[i].y, newPath[i].z);

                if (Random.Range(0.0f, 1.0f) < percentageChangeOffPath) {
                    boneLocation.x += Random.Range(0.0f, 1.0f) >= 0.5f ? Random.Range(-offPathMinimumX, -offPathMaximumX) : Random.Range(offPathMinimumX, offPathMaximumX);
                }
                bones.Add(boneLocation);

                float negativeRange = (bones.Count > 0) ? maxRandomDisplacementZ : -maxRandomDisplacementZ / 1.5f;
                float possitiveRange = (bones.Count < this.amountOfBones - 2) ? maxRandomDisplacementZ : -maxRandomDisplacementZ / 1.5f;
                randomAdder = offsetStartBoneSpawnZ + Random.Range(-negativeRange, possitiveRange);
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

    public void SetLastLevel(bool newLastLevel) {
        this.lastLevel = newLastLevel;
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
        } while (furdestLocationZ - this.StartZ < MinimumRenderDistanceZ && furdestLocationZ + ChunkLength <= this.EndZ);
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
        spawnedBoneLocation = 0;
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

            if (Mathf.Abs(x + (deltaX * 10)) > this.ChunkWidthRadius) {
                deltaX *= 0.5f;
            }

            x += deltaX;
        }

        return path;
    }

    private void SpawnChunk(float chunkStartZ) {
        if (chunkStartZ + ChunkLength <= this.EndZ) { // Prevent new chunk spawning past the map.
            if (chunkStartZ >= this.StartZ) { // Prevent new chunk spawning before the map.
                Chunk newChunk = new Chunk(Template, new Rect(-ChunkWidthRadius, chunkStartZ, ChunkWidthRadius * 2, ChunkLength));

                // Clear objects from path
                float clearRadius = 1;
                List<GameObject> spawned = newChunk.GetSpawned();

                for (int i = 0; i < spawned.Count; i++) {
                    GameObject obj = spawned[i];

                    if (obj.tag != "Ground") {
                        Collider collider = obj.GetComponent<Collider>();

                        if (collider != null) {
                            foreach (Vector3 pathPoint in path) {
                                if (Mathf.Abs(pathPoint.z - collider.bounds.center.z) < 10) {
                                    float distance = Vector3.Distance(collider.bounds.center, pathPoint);

                                    Vector3 closestPoint = collider.bounds.ClosestPoint(pathPoint);
                                    float closestDistance = Vector3.Distance(closestPoint, pathPoint);

                                    if (distance < clearRadius || closestDistance < clearRadius) {
                                        SpawnController.Destroy(obj);
                                        spawned.RemoveAt(i);
                                        i--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Add objects on path
                Random.InitState(Seed);
                int minimumFreeSpaceForBones = 4;
                int pathI = Random.Range(8, 12);
                while(pathI < path.Count) {
                    float z = pathI + path[0].z;

                    if (z >= chunkStartZ && z <= chunkStartZ + ChunkLength) {
                        for (int i = 0; i < bones.Count; i++) {
                            if(Mathf.Abs(bones[i].z - z) <= minimumFreeSpaceForBones) {
                                pathI++;
                                goto endPathLoop;
                            }
                        }

                        Vector3 pathPoint = path[pathI];
                        newChunk.SpawnRandom(new Vector3(pathPoint.x, 0f, pathPoint.z), new string[] { "Grass" });
                    }

                    pathI += Random.Range(8, 12);

                    endPathLoop: { }
                }

                // Draw path
                GameObject pathBlueprint = (GameObject)Resources.Load("Path");
                foreach (Vector3 pathPoint in path) {
                    if (newChunk.SpawnArea.Contains(new Vector2(pathPoint.x, pathPoint.z))) {
                        spawned.Add(SpawnController.Instantiate(pathBlueprint, new Vector3(pathPoint.x, 0.01f, pathPoint.z), new Quaternion()));
                    }
                }

                // Add bones to the chunk.
                Random.InitState(System.DateTime.Now.Millisecond);
                for (int i = spawnedBoneLocation; i < bones.Count; i++) {
                    Vector3 boneLocation = new Vector3(bones[i].x, bones[i].y, bones[i].z);
                    if (boneLocation.z >= chunkStartZ && boneLocation.z <= chunkStartZ + ChunkLength) {
                        boneLocation.z -= chunkStartZ;
                        SpawnController.spawnItem(newChunk, (GameObject)Resources.Load("BoneItem"), boneLocation);
                        spawnedBoneLocation++;
                    } else {
                        break;
                    }
                }

                // If last chunk add finish plane.
                if (chunkStartZ + ChunkLength == this.EndZ) {
                    SpawnController.spawnPlaneFunc(newChunk, (GameObject)Resources.Load("FinishPlane"), new Vector3(0, 0.001f, 0));
                    if (this.lastLevel) {
                        //SpawnController.spawnPlaneFunc(newChunk, (GameObject)Resources.Load("FinishPlane"), new Vector3(ChunkLength, 0.001f, 0));

                        GameObject endWall = (GameObject)Resources.Load("EndWall");
                        endWall.transform.localScale = new Vector3(2 * ChunkWidthRadius, 20, 1);
                        float endWallX = 0;
                        float endWallY = endWall.transform.localScale.y / 2;
                        float endWallZ = endWall.transform.localScale.z / 2 + furdestPlacedEdge;
                        Vector3 endWallPos = new Vector3(endWallX, endWallY, endWallZ);
                        SpawnController.Instantiate(endWall, endWallPos, Quaternion.identity);
                    }
                }

                // Add edge objects to the map.
                if (furdestPlacedEdge < chunkStartZ) {
                    // Add edges to the the chunk in just passed the last location of the edge.
                    if (rememberEdgeLeft != null && rememberEdgeRight != null) {
                        newChunk.Spawned.Add(rememberEdgeLeft);
                        newChunk.Spawned.Add(rememberEdgeRight);
                    }
                    Renderer theRenderer = edgePrefab.GetComponent<Renderer>();
                    if (theRenderer == null) {
                        theRenderer = edgePrefab.GetComponentInChildren<Renderer>();
                    }
                    float edgeWidthRadius = theRenderer.bounds.size.x / 2;
                    float edgeLength = theRenderer.bounds.size.z;
                    float offsetZ = edgeLength / 2;
                    rememberEdgeLeft = SpawnController.Instantiate(edgePrefab, new Vector3(-ChunkWidthRadius - edgeWidthRadius, 0, furdestPlacedEdge + offsetZ), Quaternion.identity);
                    rememberEdgeRight = SpawnController.Instantiate(edgePrefab, new Vector3(ChunkWidthRadius + edgeWidthRadius, 0, furdestPlacedEdge + offsetZ), Quaternion.Euler(0, 180, 0));
                    furdestPlacedEdge += edgeLength;
                }

                chunks.Add(newChunk);
            }
        }
    }
}
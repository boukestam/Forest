﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    private LevelManager levelManager;

    void Start() {
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

        levelManager = new LevelManager(new List<Level>() {
            new Level(forestChunkTemplate, 0, 100, 80),
            new Level(forestChunkTemplate2, 100, 1000, 80)
        });

        RestartCurrentLevel();
    }

    void Update() {
        levelManager.Update();
    }

    public void RestartCurrentLevel() {
        levelManager.RestartCurrentLevel();
    }
}

public class LevelManager {
    List<Level> levels;
    private int currentLevel = 0;
    private GameObject scorePanel;
    private PlayerController playerController;
    bool scoreMenu = false;

    public LevelManager(List<Level> newLevels) {
        this.levels = newLevels;
        scorePanel = GameObject.Find("ScorePanel");
        scorePanel.SetActive(false);
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void EnterScorePanel() {
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

    private void NextLevel() {
        playerController.setPoints(0);
        ExitScorePanel();
        levels[currentLevel].ClearLevel();
        currentLevel++;
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
        levels[currentLevel+1].ClearLevel();
        GameObject.FindWithTag("Player").transform.position = new Vector3(0, 0, levels[currentLevel].StartZ);
    }
}

public class Level {
    private GameObject Player;
    private static float StartDespawnZ = -10;
    private static float MinimumRenderDistanceZ = 80;
    private static float ChunkLength = 2;
    private ChunkTemplate Template;
    public float StartZ;
    private float EndZ;
    private float ChunkWidthRadius;

    private List<Chunk> chunks = new List<Chunk>();
    public List<Vector3> itemPath = new List<Vector3>();

    public Level(ChunkTemplate template, float startZ, float endZ, float chunkWidthRadius) {
        Player = GameObject.FindWithTag("Player");
        this.Template = template;
        this.StartZ = startZ;
        this.EndZ = endZ;
        this.ChunkWidthRadius = chunkWidthRadius;

        // Temporary faking a straight item path
        float length = 10;
        for (int i = 1; i < 20; i++) {
            itemPath.Add(new Vector3(0,0, length*i));
        }
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

    private void SpawnChunk(float chunkStartZ) {
        if (chunkStartZ + ChunkLength <= this.EndZ) { // Prevent new chunk spawning past the map.
            if (chunkStartZ >= this.StartZ) { // Prevent new chunk spawning before the map.
                Chunk newChunk = new Chunk(Template, new Rect(-ChunkWidthRadius, chunkStartZ, ChunkWidthRadius * 2, ChunkLength));
                chunks.Add(newChunk);

                // Add coins to the chunk.
                float chunkStartRelativeZ = chunkStartZ - this.StartZ;
                for (int i=0;i< itemPath.Count; i++) {
                    Vector3 coinLocation = itemPath[i];
                    if (coinLocation.z > chunkStartRelativeZ && coinLocation.z <= chunkStartRelativeZ + ChunkLength) {
                        coinLocation.z -= chunkStartRelativeZ;
                        SpawnController.spawnItem(newChunk, (GameObject)Resources.Load("BoneItem"), coinLocation);
                    }
                    
                }

                // If last chunk add finish plane.
                if (chunkStartZ + ChunkLength == this.EndZ) { 
                    Chunk lastChunk = chunks[chunks.Count - 1];
                    SpawnController.spawnPlaneFunc(lastChunk, (GameObject)Resources.Load("FinishPlane"), new Vector3(0, 0.001f, 0));
                }
            }
        }
    }
}
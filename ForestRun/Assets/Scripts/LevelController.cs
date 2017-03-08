using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            new Level(forestChunkTemplate, 0, 300, 80),
            new Level(forestChunkTemplate2, 300, 1000, 80)
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

    public LevelManager(List<Level> newLevels) {
        this.levels = newLevels;
    }

    public void Update() {
        // Check for going to new level.
        if (levels[currentLevel].completedLevel()) {
            levels[currentLevel].ClearLevel();
            currentLevel++;
            Debug.Log("New level!!!!");
            if (currentLevel >= levels.Count) {
                currentLevel = levels.Count - 1;
            }
        }

        levels[currentLevel].Update();
        if (currentLevel+1 < levels.Count) {
            levels[currentLevel+1].Update();
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

    public Level(ChunkTemplate template, float startZ, float endZ, float chunkWidthRadius) {
        Player = GameObject.FindWithTag("Player");
        this.Template = template;
        this.StartZ = startZ;
        this.EndZ = endZ;
        this.ChunkWidthRadius = chunkWidthRadius;
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
        return Player.transform.position.z > this.EndZ;
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

    private void SpawnChunk(float chunkZ) {
        if (chunkZ + ChunkLength <= this.EndZ) { // Prevent new chunk spawning past the map.
            if (chunkZ >= this.StartZ) { // Prevent new chunk spawning before the map.
                chunks.Add(new Chunk(Template, new Rect(-ChunkWidthRadius, chunkZ, ChunkWidthRadius * 2, ChunkLength)));
                if(chunkZ + ChunkLength == this.EndZ) { // If last chunk add finish.
                    Chunk lastChunk = chunks[chunks.Count - 1];
                    SpawnController.spawnPlaneFunc(lastChunk, (GameObject)Resources.Load("FinishPlane"), new Vector3(0, 0.001f, 0));
                }
            }
        }
    }
}
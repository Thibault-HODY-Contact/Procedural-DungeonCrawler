using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GenGenerator : MonoBehaviour
{
    [FormerlySerializedAs("GeneratorPrefab")][SerializeField] GameObject levelPrefab;
    [FormerlySerializedAs("numberLEveles")][SerializeField] int levelCount;
    [FormerlySerializedAs("Ladder")][SerializeField] GameObject ladderPrefab;
    [SerializeField] float levelSpacing = 15f;
    [FormerlySerializedAs("GenList")][SerializeField] List<GameObject> spawnedLevels = new List<GameObject>();
    [SerializeField] List<GameObject> spawnedLadders = new List<GameObject>();

    [SerializeField] private Transform player;

    [ContextMenu("Generate")]
    void GenerateLevels()
    {
        ClearLevels();

        for (int i = 0; i < levelCount; i++)
            spawnedLevels.Add(Instantiate(levelPrefab));

        foreach (GameObject level in spawnedLevels)
        {
            DungeonGeneratorBSP generator = level.GetComponent<DungeonGeneratorBSP>();
            generator.Generate();
            generator.useRandomSeed = false;
        }

        for (int i = 0; i < levelCount; i++)
        {
            DungeonGeneratorBSP generator = spawnedLevels[i].GetComponent<DungeonGeneratorBSP>();
            if (i > 0)
                StackOnPreviousLevel(generator, spawnedLevels[i - 1].GetComponent<DungeonGeneratorBSP>());

            spawnedLadders.Add(Instantiate(ladderPrefab, GetLadderPosition(generator), generator.transform.rotation));
            generator.BuildRooms();
        }
        PlacePlayer();
    }

    void StackOnPreviousLevel(DungeonGeneratorBSP level, DungeonGeneratorBSP previousLevel)
    {
        Vector2 offset = level.StartRoom.roomBounds.center - previousLevel.EndRoom.roomBounds.center;
        level.transform.position -= new Vector3(offset.x, 0f, offset.y);
        level.transform.position += new Vector3(0f, previousLevel.transform.position.y + levelSpacing, 0f);
        level.Generate();
    }

    Vector3 GetLadderPosition(DungeonGeneratorBSP level)
    {
        Vector2 endRoomCenter = level.EndRoom.roomBounds.center;
        return new Vector3(endRoomCenter.x, level.transform.position.y, endRoomCenter.y);
    }

    void ClearLevels()
    {
        foreach (GameObject level in spawnedLevels)
        {
            if (level == null) continue;
            level.GetComponent<DungeonGeneratorBSP>().ClearRoomObjects();
            DestroyImmediate(level);
        }

        foreach (GameObject ladder in spawnedLadders)
            if (ladder != null) DestroyImmediate(ladder);

        spawnedLevels.Clear();
        spawnedLadders.Clear();
    }



    private void PlacePlayer()
    {
        DungeonGeneratorBSP firstLevel = spawnedLevels[0].GetComponent<DungeonGeneratorBSP>();

        Vector2 center = firstLevel.StartRoom.roomBounds.center;

        player.position = new Vector3(
            center.x,
            firstLevel.transform.position.y + 1f,
            center.y
        );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GenGenerator : MonoBehaviour
{
    [FormerlySerializedAs("GeneratorPrefab")][SerializeField] GameObject levelPrefab;
    [FormerlySerializedAs("numberLEveles")][SerializeField] public int levelCount;
    [FormerlySerializedAs("Ladder")][SerializeField] GameObject ladderPrefab;
    [FormerlySerializedAs("End")][SerializeField] GameObject END;

    [SerializeField] float levelSpacing = 15f;
    [FormerlySerializedAs("GenList")][SerializeField] List<GameObject> spawnedLevels = new List<GameObject>();
    [SerializeField] List<GameObject> spawnedLadders = new List<GameObject>();
    [SerializeField] private GameObject gasObject;
    [SerializeField] private Transform player;
    [SerializeField] private float gasHeightOffset = 0.1f;
    [SerializeField] private float gasSpawnDelay = 5f;

    private Coroutine gasSpawnCoroutine;
    private void Awake()
    {
        GameManager.Instance.genGenerator = this.gameObject;
    }

    [ContextMenu("Generate")]
    public void GenerateLevels()
    {
        ClearLevels();

        for (int i = 0; i < levelCount; i++)
            spawnedLevels.Add(Instantiate(levelPrefab));

        spawnedLevels[0].GetComponent<DungeonGeneratorBSP>().isFirstLevel = true;
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
            if(i == levelCount-1)
            {
                DungeonGeneratorBSP level = spawnedLevels[i].GetComponent<DungeonGeneratorBSP>();
                Vector3 pos = new Vector3(level.EndRoom.roomBounds.center.x, level.transform.position.y + levelSpacing, level.EndRoom.roomBounds.center.y);

                Instantiate(END, pos, Quaternion.identity);
            }
            generator.BuildRooms();
        }
        PlacePlayer();

        if (gasSpawnCoroutine != null)
            StopCoroutine(gasSpawnCoroutine);

        if (gasObject != null)
            gasObject.SetActive(false);

        PlaceGasAtStart();

        gasSpawnCoroutine = StartCoroutine(SpawnGasAfterDelay());
    }
    public void StopGas()
    {
        if (gasSpawnCoroutine != null)
        {
            StopCoroutine(gasSpawnCoroutine);
            gasSpawnCoroutine = null;
        }

        if (gasObject == null)
            return;

        GasSpread spread = gasObject.GetComponent<GasSpread>();

        if (spread != null)
            spread.StopSpread();

        Debug.Log("[GAZ] Propagation arrêtée à l'échelle.");
    }
    private void PlaceGasAtStart()
    {
        if (gasObject == null || player == null)
            return;

        gasObject.transform.position = new Vector3(
            player.position.x,
            player.position.y - 1f + gasHeightOffset,
            player.position.z
        );
    }
    void StackOnPreviousLevel(DungeonGeneratorBSP level, DungeonGeneratorBSP previousLevel)
    {
        Vector2 offset = level.StartRoom.roomBounds.center - previousLevel.EndRoom.roomBounds.center;
        level.transform.position -= new Vector3(offset.x, 0f, offset.y);
        level.transform.position += new Vector3(0f, previousLevel.transform.position.y + levelSpacing, 0f);
        level.Generate();
    }
    private IEnumerator SpawnGasAfterDelay()
    {
        yield return new WaitForSeconds(gasSpawnDelay);

        if (gasObject == null)
            yield break;

        GasSpread gasSpread = gasObject.GetComponent<GasSpread>();

        if (gasSpread != null)
        {
            gasSpread.Initialize(player);
            gasSpread.StartSpread();
        }

        gasObject.SetActive(true);
        gasSpawnCoroutine = null;
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

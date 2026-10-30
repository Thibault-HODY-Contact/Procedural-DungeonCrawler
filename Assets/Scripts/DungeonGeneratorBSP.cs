using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DungeonGeneratorBSP : MonoBehaviour
{
    const float GizmoHeight = 5f;
    const float MarkerRadius = 2f;

    [FormerlySerializedAs("map")][SerializeField] BSPMAPSO settings;
    [FormerlySerializedAs("Seed")][SerializeField] int seed;
    [FormerlySerializedAs("RandomSeed")] public bool useRandomSeed;
    [SerializeField] int cornerRotationOffset;
    [SerializeField] float wallInset = 0.5f;
    [SerializeField] bool drawCorridorGizmos;

    List<BSPNode> rooms = new List<BSPNode>();
    List<CorridorPiece> corridors = new List<CorridorPiece>();
    readonly List<GameObject> spawnedCorridors = new List<GameObject>();
    readonly List<GameObject> spawnedFloors = new List<GameObject>();
    readonly List<GameObject> spawnedWalls = new List<GameObject>();
    readonly List<GameObject> spawnedProps = new List<GameObject>();
    public bool isFirstLevel;

    public BSPNode StartRoom { get; private set; }
    public BSPNode EndRoom { get; private set; }
    RectInt MapArea
    {
        get
        {
            Vector2Int center = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            return new RectInt(center - settings.mapSize / 2, settings.mapSize);
        }
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        ClearGeneration();
        if (useRandomSeed) seed = Random.Range(int.MinValue, int.MaxValue);
        System.Random rng = new System.Random(seed);

        BSPLayout layout = new BSPLayout(settings);
        BSPNode root = layout.Build(MapArea, transform.position, rng);
        rooms = layout.Rooms;
        StartRoom = layout.StartRoom;
        EndRoom = layout.EndRoom;

        if (isFirstLevel) 
        {
            StartRoom.style = settings.roomStyles.Find(style => style != null && !style.canBeStartRoom && !style.canBeEndRoom); 
        }

        CorridorPlanner planner = new CorridorPlanner(settings, rooms);
        corridors = planner.ConnectAllRooms(root, rng);
        RoomGraph.ComputeDistanceToExit(rooms, corridors, EndRoom);
        if (planner.FailedConnections > 0)
            Debug.LogWarning($"{planner.FailedConnections} zone(s) sans couloir libre (seed {seed}).");

        new CorridorBuilder(settings, transform, cornerRotationOffset, wallInset).SpawnAll(corridors, spawnedCorridors);
    }

    [ContextMenu("Build Rooms")]
    public void BuildRooms()
    {
        ClearRoomObjects();
        RoomBuilder builder = new RoomBuilder(transform, corridors, wallInset);
        builder.BuildFloors(rooms, spawnedFloors);
        builder.BuildWalls(rooms, spawnedWalls);
        List<Vector2> ladderSpots = new List<Vector2> { StartRoom.roomBounds.center, EndRoom.roomBounds.center };
        new PropSpawner(transform, corridors, wallInset, seed).SpawnAll(rooms, ladderSpots, spawnedProps);
    }

    public void ClearRoomObjects()
    {
        DestroyAll(spawnedFloors);
        DestroyAll(spawnedWalls);
        DestroyAll(spawnedProps);
    }

    void ClearGeneration()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
            children.Add(child.gameObject);
        DestroyAll(children);

        rooms.Clear();
        corridors.Clear();
        spawnedCorridors.Clear();
        StartRoom = null;
        EndRoom = null;
    }

    static void DestroyAll(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
            if (obj != null) DestroyImmediate(obj);
        objects.Clear();
    }

    void OnDrawGizmos()
    {
        if (settings == null) return;

        if (drawCorridorGizmos)
            foreach (CorridorPiece piece in corridors)
                DrawAreaGizmo(piece.area, piece.isCorner ? Color.red : Color.yellow);

        DrawRoomMarker(StartRoom, Color.green);
        DrawRoomMarker(EndRoom, Color.magenta);
    }

    void DrawAreaGizmo(RectInt area, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawCube(new Vector3(area.center.x, transform.position.y, area.center.y), new Vector3(area.width, GizmoHeight, area.height));
    }

    void DrawRoomMarker(BSPNode room, Color color)
    {
        if (room == null) return;
        Gizmos.color = color;
        Vector2 center = room.roomBounds.center;
        Gizmos.DrawSphere(new Vector3(center.x, transform.position.y, center.y), MarkerRadius);
    }
}
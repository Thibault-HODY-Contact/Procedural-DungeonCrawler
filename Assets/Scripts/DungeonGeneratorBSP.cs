using System.Collections.Generic;
using UnityEngine;

public class BSPNode
{
    public RectInt bounds;
    public BSPNode left, right;
    public RectInt? room;
    public RoomStyleSO style;
    public bool IsLeaf => left == null && right == null;
}

public class DungeonGeneratorBSP : MonoBehaviour
{
    const int CorridorAttempts = 10;

    [SerializeField] public BSPMAPSO map;
    [SerializeField] int Seed;
    [SerializeField] public bool RandomSeed = false;

    List<RectInt> Feuilles = new List<RectInt>();
    List<BSPNode> rooms = new List<BSPNode>();
    List<RectInt> corridors = new List<RectInt>();
    public BSPNode rootNode;
    public BSPNode startRoom;
    public BSPNode endRoom;

    RectInt MapBounds => new RectInt(new Vector2Int((int)gameObject.transform.position.x - map.MapSize.x / 2, (int)gameObject.transform.position.z - map.MapSize.y / 2), map.MapSize);

    [ContextMenu("Generate")]
    public void Generate()
    {
        rooms.Clear();
        corridors.Clear();
        Feuilles.Clear();
        startRoom = null;
        endRoom = null;

        if (RandomSeed) generateRandomSeed();
        var rng = new System.Random(Seed);

        rootNode = new BSPNode { bounds = MapBounds };
        Split(rootNode, map.MaxDepth, rng);
        PlaceRooms(rootNode, rng);
        AssignStartAndEnd(rng);
        resizeRoom();

        ConnectRooms(rootNode, rng);
    }
    void resizeRoom()
    {
        Vector2Int padding = Vector2Int.one * map.Padding;
        foreach (BSPNode node in rooms)
        { 
            node.room = new RectInt(node.bounds.position + padding, node.bounds.size - padding * 2);
        }
    }

    void Split(BSPNode node, int depth, System.Random rng)
    {
        var b = node.bounds;
        bool horizontal = b.height > b.width;
        int length = horizontal ? b.height : b.width;
        if (depth == 0 || length < map.MinNodeSize * 2) return;

        int cut = rng.Next(map.MinNodeSize, length - map.MinNodeSize + 1);
        if (horizontal)
        {
            node.left = new BSPNode { bounds = new RectInt(b.x, b.y, b.width, cut) };
            node.right = new BSPNode { bounds = new RectInt(b.x, b.y + cut, b.width, b.height - cut) };
        }
        else
        {
            node.left = new BSPNode { bounds = new RectInt(b.x, b.y, cut, b.height) };
            node.right = new BSPNode { bounds = new RectInt(b.x + cut, b.y, b.width - cut, b.height) };
        }
        Split(node.left, depth - 1, rng);
        Split(node.right, depth - 1, rng);
    }

    void PlaceRooms(BSPNode node, System.Random rng)
    {
        if (!node.IsLeaf)
        {
            PlaceRooms(node.left, rng);
            PlaceRooms(node.right, rng);
            return;
        }

        int maxW = node.bounds.width - 2 * map.Padding;
        int maxH = node.bounds.height - 2 * map.Padding;
        int w = rng.Next(map.MinRoomSize.x, maxW + 1);
        int h = rng.Next(map.MinRoomSize.y, maxH + 1);
        int x = node.bounds.x + map.Padding + rng.Next(0, maxW - w + 1);
        int y = node.bounds.y + map.Padding + rng.Next(0, maxH - h + 1);

        node.room = new RectInt(x, y, w, h);
        node.style = PickWeightedStyle(rng);

        Feuilles.Add(node.bounds);
        rooms.Add(node);
    }

    RoomStyleSO PickWeightedStyle(System.Random rng)
    {
        if (map.RoomStyles == null) return null;

        float total = 0f;
        foreach (RoomStyleSO s in map.RoomStyles)
            if (s != null) total += Mathf.Max(0f, s.Weight);
        if (total <= 0f) return null;

        double roll = rng.NextDouble() * total;
        RoomStyleSO last = null;
        foreach (RoomStyleSO s in map.RoomStyles)
        {
            if (s == null || s.Weight <= 0f) continue;
            last = s;
            roll -= s.Weight;
            if (roll < 0) return s;
        }
        return last;
    }

    RoomStyleSO PickSpecialStyle(System.Random rng, System.Predicate<RoomStyleSO> allowed)
    {
        if (map.RoomStyles == null) return null;

        List<RoomStyleSO> candidates = map.RoomStyles.FindAll(s => s != null && allowed(s));
        return candidates.Count > 0 ? candidates[rng.Next(candidates.Count)] : null;
    }

    void AssignStartAndEnd(System.Random rng)
    {
        if (rooms.Count == 0) return;

        startRoom = rooms[rng.Next(rooms.Count)];
        endRoom = startRoom;
        float best = float.MaxValue;
        foreach (BSPNode n in rooms)
        {
            Vector2Int mappos = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.z);
            float d = Vector2.Distance(n.room.Value.center, mappos);
            if (d < best) { best = d; startRoom = n; }
        }

        RoomStyleSO startStyle = PickSpecialStyle(rng, s => s.AllowAsStartRoom);
        if (startStyle != null) startRoom.style = startStyle;

        if (endRoom != startRoom)
        {
            RoomStyleSO endStyle = PickSpecialStyle(rng, s => s.AllowAsEndRoom);
            if (endStyle != null) endRoom.style = endStyle;
        }
    }

    void ConnectRooms(BSPNode node, System.Random rng)
    {
        if (node.IsLeaf) return;

        ConnectRooms(node.left, rng);
        ConnectRooms(node.right, rng);

        List<BSPNode> leftRooms = new List<BSPNode>();
        List<BSPNode> rightRooms = new List<BSPNode>();
        CollectRooms(node.left, leftRooms);
        CollectRooms(node.right, rightRooms);

        List<(BSPNode a, BSPNode b)> pairs = new List<(BSPNode a, BSPNode b)>();
        foreach (BSPNode a in leftRooms)
            foreach (BSPNode b in rightRooms)
                pairs.Add((a, b));
        pairs.Sort((p, q) => Distance(p.a, p.b).CompareTo(Distance(q.a, q.b)));

        foreach (var pair in pairs)
        {
            List<RectInt> corridor = TryCorridor(pair.a, pair.b, rng);
            if (corridor == null) continue;
            corridors.AddRange(corridor.FindAll(r => r.width > 0 && r.height > 0));
            return;
        }

        Debug.LogWarning($"Aucun couloir libre trouvé entre deux zones (seed {Seed}).");
    }

    void CollectRooms(BSPNode node, List<BSPNode> result)
    {
        if (node.IsLeaf)
        {
            result.Add(node);
            return;
        }
        CollectRooms(node.left, result);
        CollectRooms(node.right, result);
    }

    List<RectInt> TryCorridor(BSPNode a, BSPNode b, System.Random rng)
    {
        List<RectInt> straight = TryStraight(a, b, rng);
        if (straight != null) return straight;

        for (int i = 0; i < CorridorAttempts; i++)
        {
            List<RectInt> elbow = rng.Next(2) == 0 ? TryElbow(a, b, rng) : TryElbow(b, a, rng);
            if (elbow != null) return elbow;
        }
        return null;
    }

    List<RectInt> TryStraight(BSPNode a, BSPNode b, System.Random rng)
    {
        RectInt ra = a.room.Value;
        RectInt rb = b.room.Value;
        int w = map.CorridorWidth;

        int xMin = Mathf.Max(ra.xMin, rb.xMin);
        int xMax = Mathf.Min(ra.xMax, rb.xMax);
        if (xMax - xMin >= w)
        {
            bool aBelow = ra.yMax <= rb.yMin;
            RectInt low = aBelow ? ra : rb;
            RectInt high = aBelow ? rb : ra;
            for (int i = 0; i < CorridorAttempts; i++)
            {
                int x = rng.Next(xMin, xMax - w + 1);
                RectInt r = new RectInt(x, low.yMax, w, high.yMin - low.yMax);
                if (IsFree(r, a, b)) return new List<RectInt> { r };
            }
        }

        int yMin = Mathf.Max(ra.yMin, rb.yMin);
        int yMax = Mathf.Min(ra.yMax, rb.yMax);
        if (yMax - yMin >= w)
        {
            bool aLeft = ra.xMax <= rb.xMin;
            RectInt low = aLeft ? ra : rb;
            RectInt high = aLeft ? rb : ra;
            for (int i = 0; i < CorridorAttempts; i++)
            {
                int y = rng.Next(yMin, yMax - w + 1);
                RectInt r = new RectInt(low.xMax, y, high.xMin - low.xMax, w);
                if (IsFree(r, a, b)) return new List<RectInt> { r };
            }
        }

        return null;
    }

    List<RectInt> TryElbow(BSPNode from, BSPNode to, System.Random rng)
    {
        RectInt a = from.room.Value;
        RectInt b = to.room.Value;
        int w = map.CorridorWidth;

        int y = rng.Next(a.yMin, a.yMax - w + 1);
        int x = rng.Next(b.xMin, b.xMax - w + 1);
        RectInt elbow = new RectInt(x, y, w, w);

        RectInt horizontal;
        if (elbow.xMin >= a.xMax) horizontal = new RectInt(a.xMax, y, elbow.xMax - a.xMax, w);
        else if (elbow.xMax <= a.xMin) horizontal = new RectInt(elbow.xMin, y, a.xMin - elbow.xMin, w);
        else return null;

        RectInt vertical;
        if (elbow.yMax <= b.yMin) vertical = new RectInt(x, elbow.yMax, w, b.yMin - elbow.yMax);
        else if (elbow.yMin >= b.yMax) vertical = new RectInt(x, b.yMax, w, elbow.yMin - b.yMax);
        else return null;

        if (!IsFree(horizontal, from, to) || !IsFree(vertical, from, to)) return null;
        return new List<RectInt> { horizontal, vertical };
    }

    bool IsFree(RectInt r, BSPNode a, BSPNode b)
    {
        if (r.width <= 0 || r.height <= 0) return true;

        foreach (BSPNode n in rooms)
        {
            RectInt room = n.room.Value;
            if (n != a && n != b)
                room = new RectInt(room.x - 1, room.y - 1, room.width + 2, room.height + 2);
            if (r.Overlaps(room)) return false;
        }
        return true;
    }

    float Distance(BSPNode a, BSPNode b) => Vector2.Distance(a.room.Value.center, b.room.Value.center);

    int generateRandomSeed()
    {
        Seed = Random.Range(int.MinValue, int.MaxValue);
        return Seed;
    }

    private void OnDrawGizmos()
    {
        if (map == null) return;

        DrawRect(MapBounds, map.gizmoColor);

        foreach (RectInt f in Feuilles) DrawRect(f, Color.red);

        foreach (BSPNode n in rooms)
            DrawRect(n.room.Value, n.style != null ? n.style.gizmoColor : Color.white);

        foreach (RectInt c in corridors) DrawRect(c, Color.yellow);

        DrawMarker(startRoom, Color.green);
        DrawMarker(endRoom, Color.magenta);
    }

    void DrawRect(RectInt r, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawCube(new Vector3(r.center.x, (int)gameObject.transform.position.y, r.center.y), new Vector3(r.width, 5, r.height));
    }

    void DrawMarker(BSPNode node, Color color)
    {
        if (node == null) return;
        Gizmos.color = color;
        Vector2 c = node.room.Value.center;
        Gizmos.DrawSphere(new Vector3(c.x, (int)gameObject.transform.position.y, c.y), 2f);
    }
}
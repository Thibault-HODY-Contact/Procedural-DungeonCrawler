using System.Collections.Generic;
using UnityEngine;

public class BSPLayout
{
    readonly BSPMAPSO settings;
    Vector3 generatorPosition;

    public List<BSPNode> Rooms { get; } = new List<BSPNode>();
    public BSPNode StartRoom { get; private set; }
    public BSPNode EndRoom { get; private set; }

    public BSPLayout(BSPMAPSO settings)
    {
        this.settings = settings;
    }

    public BSPNode Build(RectInt mapArea, Vector3 generatorPosition, System.Random rng)
    {
        this.generatorPosition = generatorPosition;
        BSPNode root = new BSPNode { area = mapArea };
        SplitNode(root, settings.maxSplitDepth, rng);
        CreateRooms(root, rng);
        ChooseStartAndEndRooms(rng);
        ApplyRoomPadding();
        return root;
    }

    void SplitNode(BSPNode node, int remainingDepth, System.Random rng)
    {
        RectInt area = node.area;
        bool cutAcrossHeight = area.height > area.width;
        int lengthToCut = cutAcrossHeight ? area.height : area.width;
        if (remainingDepth == 0 || lengthToCut < settings.minLeafSize * 2) return;

        int cutPosition = rng.Next(settings.minLeafSize, lengthToCut - settings.minLeafSize + 1);
        if (cutAcrossHeight)
        {
            node.left = new BSPNode { area = new RectInt(area.x, area.y, area.width, cutPosition) };
            node.right = new BSPNode { area = new RectInt(area.x, area.y + cutPosition, area.width, area.height - cutPosition) };
        }
        else
        {
            node.left = new BSPNode { area = new RectInt(area.x, area.y, cutPosition, area.height) };
            node.right = new BSPNode { area = new RectInt(area.x + cutPosition, area.y, area.width - cutPosition, area.height) };
        }
        SplitNode(node.left, remainingDepth - 1, rng);
        SplitNode(node.right, remainingDepth - 1, rng);
    }

    void CreateRooms(BSPNode node, System.Random rng)
    {
        if (!node.IsLeaf)
        {
            CreateRooms(node.left, rng);
            CreateRooms(node.right, rng);
            return;
        }

        int padding = settings.roomPadding;
        int maxWidth = node.area.width - 2 * padding;
        int maxHeight = node.area.height - 2 * padding;
        int width = rng.Next(settings.minRoomSize.x, maxWidth + 1);
        int height = rng.Next(settings.minRoomSize.y, maxHeight + 1);
        int x = node.area.x + padding + rng.Next(0, maxWidth - width + 1);
        int y = node.area.y + padding + rng.Next(0, maxHeight - height + 1);

        node.roomBounds = new RectInt(x, y, width, height);
        node.style = PickRandomStyle(rng);
        Rooms.Add(node);
    }

    RoomStyleSO PickRandomStyle(System.Random rng)
    {
        if (settings.roomStyles == null) return null;

        float totalWeight = 0f;
        foreach (RoomStyleSO style in settings.roomStyles)
            if (style != null) totalWeight += Mathf.Max(0f, style.weight);
        if (totalWeight <= 0f) return null;

        double roll = rng.NextDouble() * totalWeight;
        RoomStyleSO lastValidStyle = null;
        foreach (RoomStyleSO style in settings.roomStyles)
        {
            if (style == null || style.weight <= 0f) continue;
            lastValidStyle = style;
            roll -= style.weight;
            if (roll < 0) return style;
        }
        return lastValidStyle;
    }

    RoomStyleSO PickStyleWhere(System.Random rng, System.Predicate<RoomStyleSO> condition)
    {
        if (settings.roomStyles == null) return null;

        List<RoomStyleSO> candidates = settings.roomStyles.FindAll(style => style != null && condition(style));
        return candidates.Count > 0 ? candidates[rng.Next(candidates.Count)] : null;
    }

    void ChooseStartAndEndRooms(System.Random rng)
    {
        if (Rooms.Count == 0) return;

        StartRoom = FindRoomClosestTo(new Vector2Int((int)generatorPosition.x, (int)generatorPosition.z));
        EndRoom = Rooms[rng.Next(Rooms.Count)];
        while (EndRoom == StartRoom && Rooms.Count > 1)
        {
            EndRoom = Rooms[rng.Next(Rooms.Count)];
        }
        RoomStyleSO startStyle = PickStyleWhere(rng, style => style.canBeStartRoom);
        if (startStyle != null) StartRoom.style = startStyle;

        if (EndRoom == StartRoom) return;

        RoomStyleSO endStyle = PickStyleWhere(rng, style => style.canBeEndRoom);
        if (endStyle != null) EndRoom.style = endStyle;
    }

    BSPNode FindRoomClosestTo(Vector2 point)
    {
        BSPNode closestRoom = null;
        float closestDistance = float.MaxValue;
        foreach (BSPNode room in Rooms)
        {
            float distance = Vector2.Distance(room.roomBounds.center, point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRoom = room;
            }
        }
        return closestRoom;
    }

    void ApplyRoomPadding()
    {
        Vector2Int padding = Vector2Int.one * settings.roomPadding;
        foreach (BSPNode room in Rooms)
        {
            Vector2Int size = room.area.size - padding * 2;
            size -= new Vector2Int(size.x % 2, size.y % 2);
            room.roomBounds = new RectInt(room.area.position + padding, size);
            room.floorHeight = generatorPosition.y;
        }
    }
}
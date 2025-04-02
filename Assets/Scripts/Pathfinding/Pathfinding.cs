using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : SingletonMonoBehaviour<Pathfinding>
{
    private const int MOVE_STRAIGHT_COST = 10;

    [SerializeField] private LayerMask obstaclesLayerMask;

    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;

    protected override void Awake()
    {
        base.Awake();
        gridSystem = new GridSystem<PathNode>(10, 10, 1f, (g, gridPos) => new PathNode(gridPos));
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize, (g, gridPosition) => new PathNode(gridPosition));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Vector3 worldPosition = gridSystem.GetWorldPosition(gridPosition);
                worldPosition.z = 0f; 

                Vector2 boxSize = Vector2.one * (cellSize * 0.9f);
                Collider2D hit = Physics2D.OverlapBox(worldPosition, boxSize, 0f, obstaclesLayerMask);

                if (hit != null)
                {
                    GetNode(x, y).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int y = 0; y < gridSystem.GetHeight(); y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                pathLength = currentNode.GetGCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbor in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbor)) continue;
                if (!neighbor.IsWalkable())
                {
                    closedList.Add(neighbor);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbor.GetGridPosition());
                if (tentativeGCost < neighbor.GetGCost())
                {
                    neighbor.SetCameFromPathNode(currentNode);
                    neighbor.SetGCost(tentativeGCost);
                    neighbor.SetHCost(CalculateDistance(neighbor.GetGridPosition(), endGridPosition));
                    neighbor.CalculateFCost();

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition a, GridPosition b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        return MOVE_STRAIGHT_COST * (xDistance + yDistance);
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> list)
    {
        PathNode lowest = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].GetFCost() < lowest.GetFCost())
            {
                lowest = list[i];
            }
        }
        return lowest;
    }

    private PathNode GetNode(int x, int y)
    {
        return gridSystem.GetGridObject(new GridPosition(x, y));
    }

    private List<PathNode> GetNeighbourList(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();
        GridPosition pos = node.GetGridPosition();

        if (pos.x - 1 >= 0) neighbors.Add(GetNode(pos.x - 1, pos.y)); // Left
        if (pos.x + 1 < gridSystem.GetWidth()) neighbors.Add(GetNode(pos.x + 1, pos.y)); // Right
        if (pos.y - 1 >= 0) neighbors.Add(GetNode(pos.x, pos.y - 1)); // Down
        if (pos.y + 1 < gridSystem.GetHeight()) neighbors.Add(GetNode(pos.x, pos.y + 1)); // Up

        return neighbors;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode> { endNode };
        PathNode current = endNode;

        while (current.GetCameFromPathNode() != null)
        {
            current = current.GetCameFromPathNode();
            path.Add(current);
        }

        path.Reverse();

        List<GridPosition> pathGridPositions = new List<GridPosition>();
        foreach (PathNode node in path)
        {
            pathGridPositions.Add(node.GetGridPosition());
        }

        return pathGridPositions;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition start, GridPosition end)
    {
        return FindPath(start, end, out int _) != null;
    }

    public int GetPathLength(GridPosition start, GridPosition end)
    {
        FindPath(start, end, out int length);
        return length;
    }
}

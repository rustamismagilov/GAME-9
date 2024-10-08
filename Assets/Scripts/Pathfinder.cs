using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] Vector2Int startCoordinates;
    [SerializeField] Vector2Int endCoordinates;

    Node currentSearchNode;
    Node startNode;
    Node endNode;

    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();


    public Vector2Int StartCoordinates
    {
        get
        {
            return startCoordinates;
        }
    }

    public Vector2Int EndCoordinates
    {
        get
        {
            return endCoordinates;
        }
    }

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();

        if (gridManager != null)
        {
            grid = gridManager.Grid;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startNode = gridManager.Grid[startCoordinates];
        endNode = gridManager.Grid[endCoordinates];

        GetNewPath();
    }

    void ExploreNeighbours()
    {
        List<Node> neighbours = new List<Node>();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighboursCoordinates = currentSearchNode.coordinates + direction;

            if (grid.ContainsKey(neighboursCoordinates))
            {
                neighbours.Add(grid[neighboursCoordinates]);

                grid[neighboursCoordinates].isExplored = true;
                grid[currentSearchNode.coordinates].isPath = true;
            }
        }

        foreach (Node neigbour in neighbours)
        {
            if (!reached.ContainsKey(neigbour.coordinates) && neigbour.isWalkable)
            {
                neigbour.connectedTo = currentSearchNode;
                reached.Add(neigbour.coordinates, neigbour);
                frontier.Enqueue(neigbour);
            }
        }
    }

    void BreadthFirstSearch(Vector2Int coordinates)
    {
        frontier.Clear();
        reached.Clear();

        bool isRunning = true;

        frontier.Enqueue(grid[coordinates]);
        reached.Add(coordinates, grid[coordinates]);

        while (frontier.Count > 0 && isRunning)
        {
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true;
            ExploreNeighbours();
            if (currentSearchNode.coordinates == endCoordinates)
            {
                isRunning = false;
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        path.Add(currentNode);
        currentNode.isPath = true;

        while (currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            path.Add(currentNode);
            currentNode.isPath = true;
        }

        path.Reverse();

        return path;
    }

    public List<Node> GetNewPath(Vector2Int coordinates)
    {
        gridManager.ResetNodes();
        BreadthFirstSearch(coordinates);
        return BuildPath();
    }

    public List<Node> GetNewPath()
    {
        return GetNewPath(startCoordinates);
    }

    public bool WillBlockPath(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            bool previousState = grid[coordinates].isWalkable;

            grid[coordinates].isWalkable = false;

            List<Node> newPath = GetNewPath();

            grid[coordinates].isWalkable = previousState;

            if (newPath.Count <= 1)
            {
                GetNewPath();
                return true;
            }
        }

        return false;
    }

    public void NotifyReceivers()
    {
        BroadcastMessage("RecalculatePath", SendMessageOptions.DontRequireReceiver);
    }
}

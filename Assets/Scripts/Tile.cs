using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] bool isPlaceable;
    [SerializeField] bool isWalkable = true;
    [SerializeField] Tower towerPrefab;

    GridManager gridManager;
    Pathfinder pathfinder;

    Vector2Int coordinates = new Vector2Int();

    public bool IsPlaceable
    {
        get 
        { 
            return isPlaceable; 
        }
    }

    public bool IsWalkable
    {
        get
        {
            return isWalkable;
        }
    }

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    void Start()
    {
        if (gridManager != null)
        {
            coordinates = gridManager.GetCoordinatesFromPosition(transform.position);

            if (!isPlaceable)
            {
                gridManager.BlockNode(coordinates);
            }
            else if (!isWalkable)
            {
                gridManager.BlockNode(coordinates);
            }
        }
    }

    void OnMouseDown()
    {
        Node node = gridManager.GetNode(coordinates);

        if (node.isWalkable && !pathfinder.WillBlockPath(coordinates))
        {
            bool isSuccessful = towerPrefab.CreateTower(towerPrefab, transform.position);
            
            if (isSuccessful)
            {
                gridManager.BlockNode(coordinates);
                pathfinder.NotifyReceivers();
            }
        }
    }
}

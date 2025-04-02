using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : SingletonMonoBehaviour<LevelGrid>
{
    [SerializeField] private Transform debugPrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    private GridSystem<GridObject> gridSystem;

    protected override void Awake()
    {
        base.Awake();
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (grid, gridPosition) => new GridObject(grid, gridPosition));
        gridSystem.CreateDebugObject(debugPrefab);
    }

    public Vector2 GetWorldPosition(GridPosition gridPosition)
    {
        return gridSystem.GetWorldPosition(gridPosition);
    }

    public GridPosition GetGridPosition(Vector2 worldPosition)
    {
        return gridSystem.GetGridPosition(worldPosition);
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridSystem.IsValidGridPosition(gridPosition);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
    }
}

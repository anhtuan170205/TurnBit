using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int moveRange = 5;

    private List<Vector3> positionList;
    private int currentPositionIndex;
    private Coroutine moveCoroutine;

    public override string GetActionName()
    {
        return "Move";
    }

    public override void ExecuteAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in gridPositionList)
        {
            Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(pathGridPosition);
            worldPos.z = 0f;
            positionList.Add(worldPos);
        }

        ActionStart(onActionComplete);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveStepByStepCoroutine());
    }

    private IEnumerator MoveStepByStepCoroutine()
    {
        while (currentPositionIndex < positionList.Count)
        {
            Vector3 targetPosition = positionList[currentPositionIndex];

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;

            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
            unit.SetGridPosition(newGridPosition);

            currentPositionIndex++;
            yield return new WaitForSeconds(0.1f);
        }

        ActionComplete();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -moveRange; x <= moveRange; x++)
        {
            for (int y = -moveRange; y <= moveRange; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                if (testGridPosition == unitGridPosition)
                {
                    continue;
                }
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }
                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }
                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }
                int pathFindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) >= moveRange * pathFindingDistanceMultiplier)
                {
                    continue;
                }
                validActionGridPositionList.Add(testGridPosition);
            }
        }
        return validActionGridPositionList;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;

            GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(mouseWorldPosition);

            MoveAction moveAction = unit.GetAction<MoveAction>();
            List<GridPosition> validPositions = moveAction.GetValidActionGridPositionList();

            if (validPositions.Contains(targetGridPosition))
            {
                moveAction.ExecuteAction(targetGridPosition, () => {
                    Debug.Log("Move Completed!");
                });
            }
            else
            {
                Debug.Log("Invalid move target!");
            }
        }
    }
}

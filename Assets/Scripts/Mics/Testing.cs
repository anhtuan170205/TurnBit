using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform unitPrefab;
    private Vector3 mousePosition;

    private void Start()
    {
        unitPrefab.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mousePosition.z = 0;
            int mouseX = Mathf.RoundToInt(mousePosition.x);
            int mouseY = Mathf.RoundToInt(mousePosition.y);
            unitPrefab.transform.position = LevelGrid.Instance.GetWorldPosition(new GridPosition(mouseX, mouseY));
        }
    }
}

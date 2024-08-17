using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

public class GenerateInfluence : MonoBehaviour
{
    //[SerializeField] private float propagationSpeed = 3f;
    [SerializeField] private int testInfluenceValue;
    [SerializeField] private int testPlayer;
    private Renderer groundRenderer;
    private int player;
    private int influenceValue;

    private void setInfluence(Vector2 cellPos, int value)
    {
        GridCell cell = LevelManager.Instance.GridController.Cells[Mathf.FloorToInt(cellPos.x), Mathf.FloorToInt(cellPos.y)];
        cell.Influences[player] = value;
    }

    private void setInfluence(GridCell cell, int value)
    {
        cell.Influences[player] = value;
    }

    private void SpreadInfluence()
    {
        SpreadInfluence(influenceValue);
    }

    private void SpreadInfluence(int radius)
    {
        Vector2 cellPos = transform.position;
        cellPos.y += radius; 
        cellPos.x = Mathf.Clamp(cellPos.x, 0, 10);
        cellPos.y = Mathf.Clamp(cellPos.y, 0, 20);
        for (int y = radius; y > 0; y--)
        {
            int value = 1;
            for (int x = 0; x < radius; x++)
            {
                setInfluence(new Vector2(cellPos.x+x,cellPos.y+y), value);
            }
            value++;
        }
    }

    private void Start()
    {
        if(testInfluenceValue != 0)
        {
            influenceValue = testInfluenceValue;
        }
        BuildingInformation buildingInfo = gameObject.GetComponent<BuildingInformation>();
        influenceValue = buildingInfo.InfluenceValue;
        SpreadInfluence();
        
    }

    private void Update()
    {
        
    }
}

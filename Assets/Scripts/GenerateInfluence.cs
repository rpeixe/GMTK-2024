using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateInfluence : MonoBehaviour
{
    //[SerializeField] private float propagationSpeed = 3f;
    [SerializeField] private int testInfluenceValue;
    [SerializeField] private int testPlayer;
    private Renderer groundRenderer;
    private int player;
    private int influenceValue;

    private void AddInfluence(Vector2Int cellPos, int value)
    {
        int x = Mathf.FloorToInt(cellPos.x);
        int y = Mathf.FloorToInt(cellPos.y);
        GridCell cell = LevelManager.Instance.GridController.Cells[x, y];
        cell.Influences[player] += value;
    }

    private void AddInfluence(GridCell cell, int value)
    {
        cell.Influences[player] += value;
    }

    private void SpreadInfluence()
    {
        SpreadInfluence(influenceValue);
    }

    private void SpreadInfluence(int radius)
    {
        Vector2 pos = transform.position;
        Vector2Int cellPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        
        cellPos.y += radius; 
        cellPos.x = Mathf.Clamp(cellPos.x, 0, 10);
        cellPos.y = Mathf.Clamp(cellPos.y, 0, 20);

        for (int y = radius; y > 0; y--)
        {
            int value = 1;
            for (int x = 0; x < radius; x++)
            {
                AddInfluence(new Vector2Int(cellPos.x+x,cellPos.y+y), value);
            }
            value++;
        }
    }

    private void Start()
    {
        if(testInfluenceValue != 0 || testPlayer != 0)
        {
            player = testPlayer;
            influenceValue = testInfluenceValue;
        }
       // BuildingInformation buildingInfo = gameObject.GetComponent<BuildingInformation>();
       // influenceValue = buildingInfo.InfluenceValue;
        SpreadInfluence();
        
    }

    private void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int Owner { get; set; } = 0;
    public Vector2Int Pos { get; set; }

    [SerializeField] private BuildingInformation info;

    public void Build()
    {
        LevelManager.Instance.RemoveCurrency(Owner, 1);
        LevelManager.Instance.GridController.SetBuilding(Pos, this);
    }

    public void Sell()
    {
        LevelManager.Instance.AddCurrency(Owner, 1);
        LevelManager.Instance.GridController.SetBuilding(Pos, null);
        Destroy(gameObject);
    }

    public void Upgrade()
    {

    }

    public void ProcessTick()
    {

    }
}

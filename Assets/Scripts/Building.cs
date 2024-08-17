using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public int Owner { get; set; } = 0;
    public Vector2Int Pos { get; set; }

    public abstract void Build();

    public void Sell()
    {
        LevelManager.Instance.AddCurrency(Owner, 1);
        LevelManager.Instance.GridController.SetBuilding(Pos, null);
        Destroy(gameObject);
    }

    public abstract void Upgrade();

    public abstract void ProcessTick();
}

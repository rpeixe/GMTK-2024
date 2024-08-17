using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GridController _gridController;
    [SerializeField] private int _numPlayers = 1;
    [SerializeField] private float _initialMoney = 10f;

    public static LevelManager Instance { get; private set; }
    public GridController GridController => _gridController;
    public Dictionary<int, float> Currencies { get; set; } = new Dictionary<int, float>();
    public Dictionary<int, int> NumBuildings { get; set; } = new Dictionary<int, int>();

    public void AddCurrency(int player, float currency)
    {
        Currencies[player] += currency;
    }

    public void RemoveCurrency(int player, float currency)
    {
        Currencies[player] -= currency;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 1; i <= _numPlayers; i++)
        {
            Currencies[i] = _initialMoney;
            NumBuildings[i] = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BuildingInformation")]
public class BuildingInformation : ScriptableObject
{
    [SerializeField] private TileBase _tile;
    [SerializeField] private string _name;
    [SerializeField] private float _baseCost;
    [SerializeField] private int _buildingTime;
    [SerializeField] private int _influenceRadius;
    [SerializeField] private float _influenceValue;
    [SerializeField] private float _income;
    [SerializeField] private BuildingInformation _evolution;
    [TextArea][SerializeField] private string _description;

    public TileBase Tile => _tile;
    public string Name => _name;
    public float BaseCost => _baseCost;
    public int BuildingTime => _buildingTime;
    public int InfluenceRadius => _influenceRadius;
    public float InfluenceValue => InfluenceValue;
    public float Income => _income;
    public string Description => _description;
    public BuildingInformation Evolution => _evolution;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingInfo")]
public class BuildingInformation : ScriptableObject
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _name;
    [SerializeField] private float _baseCost;
    [SerializeField] private int _buildingTime;
    [SerializeField] private int _influenceRadius;
    [SerializeField] private int _influenceValue;
    [SerializeField] private float _income;
    [SerializeField] private int _antiInfluenceRadius;
    [SerializeField] private int _antiInfluenceValue;
    [TextArea][SerializeField] private string _description;

    public Sprite Sprite => _sprite;
    public string Name => _name;
    public float BaseCost => _baseCost;
    public int BuildingTime => _buildingTime;
    public int InfluenceRadius => _influenceRadius;
    public int InfluenceValue => _influenceValue;
    public float Income => _income;
    public int AntiInfluenceRadius => _antiInfluenceRadius;
    public int AntiInfluenceValue => _antiInfluenceValue;
    public string Description => _description;
}

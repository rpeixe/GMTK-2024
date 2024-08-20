using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level2Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private BuildingInformation _monument;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float _timeLimitSeconds;

    private int _monumentsCaptured = 0;
    private float _timeRemaining;

    public void InitializeLevel()
    {
        LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[16,5], _hq, true, true);
        Building monument1 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[10,5], _monument, true, true);
        Building monument2 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[5,3], _monument, true, true);
        Building monument3 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[4,7], _monument, true, true);

        monument1.OnBuildingCaptured += HandleMonumentCaptured;
        monument2.OnBuildingCaptured += HandleMonumentCaptured;
        monument3.OnBuildingCaptured += HandleMonumentCaptured;

        _timeRemaining = _timeLimitSeconds;
    }

    private void HandleMonumentCaptured(Building building, int oldOwner, int newOwner)
    {
        _monumentsCaptured++;
        if (_monumentsCaptured == 3)
        {
            LevelManager.Instance.Victory();
        }
    }

    private void Update()
    {
        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0 )
        {
            _timeRemaining = 0;
            LevelManager.Instance.Defeat();
        }

        _timerText.text = TimeSpan.FromSeconds(_timeRemaining).ToString("mm':'ss");
    }
}

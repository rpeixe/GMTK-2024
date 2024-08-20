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
    [SerializeField] private GameObject _dialogueBox;

    private int _monumentsCaptured = 0;
    private float _timeRemaining;
    private Dialogue d;

    public void Dialogue()
    {
        d = _dialogueBox.GetComponent<Dialogue>();
        d.InitDialog();
        d.lines = new (int, string)[3];
        d.lines[0] = (2, "Hey, you. Nice Job at Jeju Island. Can you do the same here? I will invest in your company.");
        d.lines[1] = (0, "Wow, three monuments in Bruges, and with support. I am on it!");
        d.lines[2] = (0, "GOAL: Influence the three monuments in Bruges. This time, the investor is in a hurry.");
        d.StartDialogue();
    }
    public void InitializeLevel()
    {
        Dialogue();
        d.Unpause();
        LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[16,5], _hq, true, true);
        Building monument1 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[10,5], _monument, true, true);
        Building monument2 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[5,3], _monument, true, true);
        Building monument3 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[4,7], _monument, true, true);

        Building.OnBuildingCaptured += HandleMonumentCaptured;

        _timeRemaining = _timeLimitSeconds;
    }

    private void HandleMonumentCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type != BuildingInformation.BuildingType.monument) return;

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

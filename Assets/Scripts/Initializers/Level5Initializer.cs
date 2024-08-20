using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level5Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private BuildingInformation _monument;
    [SerializeField] private GameObject _dialogueBox;
    private Dialogue d;

    private int _player1Monuments = 0;
    private int _player2Monuments = 0;

    public void Dialogue()
    {
        d = _dialogueBox.GetComponent<Dialogue>();
        d.InitDialog();
        d.lines = new (int, string)[3];
        d.lines[0] = (2, "Now it is time to prove yourself. Go and take out your rival's main HQ.");
        d.lines[1] = (0, "Sounds hot here in Florianopolis. I will do it.");
        d.lines[2] = (0, "GOAL: Influence your rival HQ or the two monumnets before she does.");
        d.StartDialogue();
    }

    public void InitializeLevel()
    {
        Dialogue();
        Building hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[16,4], _hq, true, true);
        Building hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[4,15], _hq, true, true);

        Building monument1 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[2, 7], _monument, true, true);
        Building monument2 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[12, 17], _monument, true, true);

        Building.OnBuildingCaptured += HandleBuildingCaptured;


        Dictionary<int, Building> hqDicts = new Dictionary<int, Building>();
        hqDicts[1] = hq1;
        hqDicts[2] = hq2;

        AIManager ai1 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai1.setPlayers(2);
        ai1.SetHQs(hqDicts);
        ai1.SetGoals(new List<Building>() { monument2, monument1, hq1 });

        ai1.AddBuilding(hq1);
        ai1.AddBuilding(hq2);
        ai1.AddBuilding(monument1);
        ai1.AddBuilding(monument2);

        ai1.StartPlaying();
    }
    private void HandleBuildingCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type == BuildingInformation.BuildingType.monument)
        {
            HandleMonumentCaptured(building, oldOwner, newOwner);
        }
        else if (building.BuildingInformation.Type == BuildingInformation.BuildingType.hq)
        {
            HandleHqCaptured(building, oldOwner, newOwner);
        }
    }

    private void HandleMonumentCaptured(Building building, int oldOwner, int newOwner)
    {
        if (oldOwner == 1)
        {
            _player1Monuments--;
        }
        else if (oldOwner == 2)
        {
            _player2Monuments--;
        }

        if (newOwner == 1)
        {
            _player1Monuments++;
        }
        else if (newOwner == 2)
        {
            _player2Monuments++;
        }

        if (_player1Monuments == 2)
        {
            LevelManager.Instance.Victory();
        }
        else if (_player2Monuments == 2)
        {
            LevelManager.Instance.Defeat();
        }
    }

    private void HandleHqCaptured(Building building, int oldOwner, int newOwner)
    {
        if (oldOwner == 1)
        {
            LevelManager.Instance.Defeat();
        }
        else if (oldOwner == 2)
        {
            LevelManager.Instance.Victory();
        }
    }
}

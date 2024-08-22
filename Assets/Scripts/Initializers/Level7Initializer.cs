using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level7Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private BuildingInformation _hq3;
    [SerializeField] private BuildingInformation _monument;
    [SerializeField] private GameObject _dialogueBox;

    private int _playerMonuments = 0;
    private Dialogue d;
    public void Dialogue()
    {
        d = _dialogueBox.GetComponent<Dialogue>();
        d.InitDialog();
        d.lines = new (int, string)[3];
        d.lines[0] = (2, "I have bribed the only low - cost building here, as they are too loyal to your rival.");
        d.lines[1] = (0, "Ok, ok, got it. Maximum disadvantage requires a quick job.");
        d.lines[2] = (0, "GOAL: Get all monuments or your rival's last base.");
        d.StartDialogue();
    }

        public void InitializeLevel()
    {
        Dialogue();
        Building hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[17,18], _hq, true, true);
        Building hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[7,7], _hq3, true, true);

        Building monument1 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[11, 11], _monument, true, true);
        Building monument2 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[12, 7], _monument, true, true);
        Building monument3 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[11, 2], _monument, true, true);
        Building monument4 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[6, 1], _monument, true, true);
        Building monument5 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[1, 1], _monument, true, true);
        Building monument6 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[1, 7], _monument, true, true);
        Building monument7 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[2, 12], _monument, true, true);
        Building monument8 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[7, 13], _monument, true, true);

        Building.OnBuildingCaptured += HandleBuildingCaptured;


        Dictionary<int, Building> hqDicts = new Dictionary<int, Building>();
        hqDicts[1] = hq1;
        hqDicts[2] = hq2;

        AIManager ai1 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai1.setPlayers(2);
        ai1.SetHQs(hqDicts);
        ai1.SetGoals(new List<Building>() { monument5, monument6, monument4, monument7, monument3, monument8, monument2, monument1, hq1 });

        ai1.AddBuilding(hq1);
        ai1.AddBuilding(hq2);
        ai1.AddBuilding(monument1);
        ai1.AddBuilding(monument2);
        ai1.AddBuilding(monument3);
        ai1.AddBuilding(monument4);
        ai1.AddBuilding(monument5);
        ai1.AddBuilding(monument6);
        ai1.AddBuilding(monument7);
        ai1.AddBuilding(monument8);

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
        if (newOwner == 1)
        {
            _playerMonuments++;
        }
        if (oldOwner == 1)
        {
            _playerMonuments--;
        }
        if (_playerMonuments == 8)
        {
            LevelManager.Instance.Victory();
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
    private void OnDisable()
    {
        Building.OnBuildingCaptured -= HandleBuildingCaptured;
    }
}

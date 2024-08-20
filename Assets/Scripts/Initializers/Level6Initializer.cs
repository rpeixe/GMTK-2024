using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level6Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private BuildingInformation _hq10;
    [SerializeField] private BuildingInformation _monument;
    [SerializeField] private GameObject _dialogueBox;
    private Dialogue d;

    private int _player1Hqs = 1;
    private int _player2Hqs = 1;
    private int _player3Hqs = 1;
    public void Dialogue()
    {
        d = _dialogueBox.GetComponent<Dialogue>();
        d.InitDialog();
        d.lines = new (int, string)[2];
        d.lines[0] = (1, "Bring it on. Do you think you got it? Now try beating me and my ally.");
        d.lines[1] = (0, "Minamitorishima triangles me up, I mean… you got it.");
        d.StartDialogue();
    }
    public void InitializeLevel()
    {
        Dialogue();
        Building hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[13, 1], _hq, true, true);
        Building hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[17, 17], _hq, true, true);
        Building hq3 = LevelManager.Instance.ConstructBuilding(3, LevelManager.Instance.GridController.Cells[1, 13], _hq, true, true);

        Building monument = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[10, 10], _monument, true, true);

        Building.OnBuildingCaptured += HandleHqCaptured;


        Dictionary<int, Building> hqDicts = new Dictionary<int, Building>();
        hqDicts[1] = hq1;
        hqDicts[2] = hq2;
        hqDicts[3] = hq3;

        AIManager ai1 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai1.setPlayers(2);
        ai1.SetHQs(hqDicts);
        ai1.SetGoals(new List<Building>() { monument, hq1 });
        ai1.AddBuilding(hq1);
        ai1.AddBuilding(hq2);
        ai1.AddBuilding(hq3);
        ai1.AddBuilding(monument);

        AIManager ai2 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai2.setPlayers(3);
        ai2.SetHQs(hqDicts);
        ai2.SetGoals(new List<Building>() { monument, hq1 });
        ai2.AddBuilding(hq1);
        ai2.AddBuilding(hq2);
        ai2.AddBuilding(hq3);
        ai2.AddBuilding(monument);

        ai1.StartPlaying();
        ai2.StartPlaying();
    }

    private void HandleHqCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type != BuildingInformation.BuildingType.hq) return;

        if (oldOwner == 1)
        {
            _player1Hqs--;
        }
        else if (oldOwner == 2)
        {
            _player2Hqs--;
        }
        else if (oldOwner == 3)
        {
            _player3Hqs--;
        }

        if (newOwner == 1)
        {
            _player1Hqs++;
        }
        else if (newOwner == 2)
        {
            _player2Hqs++;
        }
        else if (newOwner == 3)
        {
            _player3Hqs++;
        }

        if (_player1Hqs == 0)
        {
            LevelManager.Instance.Defeat();
        }
        else if (_player1Hqs == 3)
        {
            LevelManager.Instance.Victory();
        }
    }

    private void HandleHqUpgraded(Building building, int owner)
    {
        if (building.BuildingInformation == _hq10)
        {
            if (owner == 1)
            {
                LevelManager.Instance.Victory();
            }
            else
            {
                LevelManager.Instance.Defeat();
            }
        }
    }
}

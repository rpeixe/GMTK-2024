using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level4Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private GameObject _dialogueBox;
    private Dialogue d;
    public void Dialogue()
    {
        d = _dialogueBox.GetComponent<Dialogue>();
        d.InitDialog();
        d.lines = new (int, string)[3];
        d.lines[0] = (1, "I didn't like a bit what you did to me. Now is the time for my revenge.");
        d.lines[1] = (0, "Ok, no monuments! That is a death match in the old west style.");
        d.lines[2] = (0, "Goal: Influence your rival HQ in Mjoifjordur.");
        d.StartDialogue();
    }
    public void InitializeLevel()
    {
        Dialogue();
        Building hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[14,4], _hq, true, true);
        Building hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[5,14], _hq, true, true);


        Dictionary<int, Building> hqDicts = new Dictionary<int, Building>();
        hqDicts[1] = hq1;
        hqDicts[2] = hq2;

        Building.OnBuildingCaptured += HandleHqCaptured;

        AIManager ai1 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai1.setPlayers(2);
        ai1.SetHQs(hqDicts);
        ai1.SetGoals(new List<Building>() { hq1 });

        ai1.AddBuilding(hq1);
        ai1.AddBuilding(hq2);

        ai1.StartPlaying();
    }

    private void HandleHqCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type != BuildingInformation.BuildingType.hq) return;

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

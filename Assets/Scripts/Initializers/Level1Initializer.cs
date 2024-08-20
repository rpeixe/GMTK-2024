using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] BuildingInformation _hq;
    [SerializeField] BuildingInformation _monument;
    [SerializeField] GameObject _dialogueBox;
    private Dialogue d;
    public void Dialogue()
    {
        d = _dialogueBox.GetComponent<Dialogue>();
        d.InitDialog();
        d.lines = new (int, string)[2];
        d.lines[0] = (0, "Man, the company is dead. There is no way we can survive this market crash!");
        d.lines[1] = (0, "Hold on a sec. Maybe I can do something on this Island.");
        d.StartDialogue();
        
    }
    public void InitializeLevel()
    {
        Dialogue();
        Building monument = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[6,5], _monument, true, true);
        LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[12,3], _hq, true, true);

        Building.OnBuildingCaptured += HandleMonumentCaptured;
    }

    private void HandleMonumentCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type != BuildingInformation.BuildingType.monument) return;

        LevelManager.Instance.Victory();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _buildMenu;
    [SerializeField] private GameObject _buildWheel;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void OpenBuildMenu(GridCell cell)
    {
        _buildMenu.SetActive(true);
        _buildWheel.GetComponent<RectTransform>().position = Input.mousePosition;
        LevelManager.Instance.Selected = cell;
    }

    public void Unselect()
    {
        LevelManager.Instance.Selected = null;
        _buildMenu.SetActive(false);
    }
}
